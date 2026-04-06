using System.Diagnostics.CodeAnalysis;
using System.Text;
using ShellProgressBar;
using Worldpack.NBT;
using Worldpack.Util;
namespace Worldpack.Tasks
{
    [WorldpackExtension]
    class StatGatherer : INBTVisitor
    {
        readonly string[] directives = [];
        readonly Dictionary<Tag, int> Counts = [];
        readonly Dictionary<Tag, int> ListCounts = [];
        readonly Dictionary<Tag, int> ListElemCounts = [];
        readonly int[] ByteSizeHistogram = new int[32];
        readonly int[] IntSizeHistogram = new int[32];
        readonly int[] LongSizeHistogram = new int[32];
        readonly HashSet<string> AllCompoundKeys = [];
        readonly HashSet<string> AllStrings = [];
        readonly Dictionary<Shape,int> AllShapes = [];
        readonly Stack<string> ListTagNames = [];
        readonly Dictionary<string, HashSet<Shape>> ShapesByListName = [];
        int CompoundKeys = 0;
        int ByteArraySize = 0;
        int IntArraySize = 0;
        int LongArraySize = 0;
        public StatGatherer(string[]? opts)
        {
            directives = opts ?? [];
            foreach (var e in Enum.GetValues<Tag>())
            {
                Counts[e] = 0;
                ListCounts[e] = 0;
                ListElemCounts[e] = 0;
            }
        }
        [WorldpackCommand(Help = """
        Computes and prints statistics of given input
        """,
        Name = "stats"
        )]
        public static void Go(string[] args,IProgressBar progress)
        {
            var infile = args[0];
            var outfile = args[1];

            StatGatherer self = new(args[2..]);

            var NBT = ITask.ReadNBT(infile, progress);
            progress.ReportMem();
            var p = progress.Spawn(1, "Gathering stats");
            var ofstream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            self.Visit(NBT);
            int TotalSize = NBT.Sizeof();
            progress.ReportMem();
            p.Tick();
            ofstream.Write($"Total Size: {TotalSize}\n");
            ofstream.Write(self.ToString()!);
            ofstream.Close();
            Thread.Sleep(1000);
        }
        public VisitDisposition Visit(CompoundTag t, string name)
        {
            Counts[Tag.Compound]++;
            CompoundKeys += t.Value.Count;
            AllCompoundKeys.Add(name);
            Shape s = new(t);
            if (!AllShapes.TryAdd(s, 0))
            {
                AllShapes[s]++;
            }
            if (name == "")
            {

            }
            return VisitDisposition.VisitAllChildren;
        }
        public VisitDisposition Visit(ListTag t, string name)
        {
            ListTagNames.Push(name);
            Counts[Tag.List]++;
            ListCounts[t.ContentTag]++;
            ListElemCounts[t.ContentTag] += t.Value.Count;
            AllCompoundKeys.Add(name);
            return VisitDisposition.VisitAllChildren;
        }
        public void PostVisit(ListTag t, string name)
        {
            ListTagNames.Pop();
        }
        public void Visit(ByteTag t, string name)
        {
            Counts[Tag.Byte]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(ShortTag t, string name)
        {
            Counts[Tag.Short]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(IntTag t, string name)
        {
            Counts[Tag.Int]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(LongTag t, string name)
        {
            Counts[Tag.Long]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(FloatTag t, string name)
        {
            Counts[Tag.Float]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(DoubleTag t, string name)
        {
            Counts[Tag.Double]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(ByteArrayTag t, string name)
        {
            Counts[Tag.ByteArray]++;
            ByteArraySize += t.Value.Length;
            var bin = int.Log2(t.Value.Length);
            ByteSizeHistogram[bin]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(StringTag t, string name)
        {
            Counts[Tag.String]++;
            AllCompoundKeys.Add(name);
            AllStrings.Add(t.Value);
        }
        public void Visit(IntArrayTag t, string name)
        {
            Counts[Tag.IntArray]++;
            IntArraySize += t.Value.Length;
            var bin = int.Log2(t.Value.Length);
            IntSizeHistogram[bin]++;
            AllCompoundKeys.Add(name);
        }
        public void Visit(LongArrayTag t, string name)
        {
            Counts[Tag.LongArray]++;
            LongArraySize += t.Value.Length;
            var bin = int.Log2(t.Value.Length);
            LongSizeHistogram[bin]++;
            AllCompoundKeys.Add(name);
        }
        public override string ToString() => $"{ReportCounts()}\n{ReportKeys()}\n{ReportShapes()}";
        string ReportCounts()
        {
            var b = new StringBuilder();
            var listb = new StringBuilder();
            b.Append("Tag Counts:\n");
            listb.Append("List Type Counts:\n");
            foreach (var t in Enum.GetValues<Tag>())
            {
                var nm = Enum.GetName(t);
                b.Append($"{nm}: {Counts[t]}\n");
                listb.Append($"{nm}: {ListCounts[t]} containing {ListElemCounts[t]}\n");

            }
            b.Append(listb);
            b.Append($"{CompoundKeys} total keys\n");
            b.Append($"{ByteArraySize} items in byte arrays\n");
            b.Append("Histogram\n");
            for (int i = 0; i < 32; i++)
            {
                if (ByteSizeHistogram[i] == 0) continue;
                b.Append($"{1 << i}: {ByteSizeHistogram[i]}\n");
            }
            b.Append($"{IntArraySize} items in int arrays\n");
            b.Append("Histogram\n");
            for (int i = 0; i < 32; i++)
            {
                if (IntSizeHistogram[i] == 0) continue;
                b.Append($"{1 << i}: {IntSizeHistogram[i]}\n");
            }
            b.Append($"{LongArraySize} items in long arrays\n");
            b.Append("Histogram\n");
            for (int i = 0; i < 32; i++)
            {
                if (LongSizeHistogram[i] == 0) continue;
                b.Append($"{1 << i}: {LongSizeHistogram[i]}\n");
            }
            return b.ToString();
        }
        string ReportKeys()
        {
            var b = new StringBuilder();
            b.Append($"{AllCompoundKeys.Count} unique keys\n");
            if (directives.Contains("+keys"))
            {
                b.Append("All Compound Keys\n");
                foreach (var k in AllCompoundKeys)
                {
                    b.Append(k);
                    b.Append('\n');
                }
            }
            b.Append($"{AllStrings.Count} unique strings\n");
            if (!directives.Contains("-strings"))
            {
                b.Append("All Strings\n");
                foreach (var s in AllStrings)
                {
                    b.Append(s);
                    b.Append('\n');
                }
            }
            return b.ToString();
        }
        string ReportShapes()
        {
            var b = new StringBuilder();
            b.Append($"{AllShapes.Count} unique shapes\n");
            foreach (var s in AllShapes)
            {
                b.Append($"{s.Value} * {s.Key}");
            }
            return b.ToString();
        }

    }
}