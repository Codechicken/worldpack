using System.Buffers;
using ShellProgressBar;
using Worldpack.Util;
namespace Worldpack.Tasks
{
    [WorldpackExtension]
    struct ToTextFormat
    {
        [WorldpackCommand(Name = "to_text", Help = """
        Takes a NBT or region file and outputs a YAML representation of its contents
        """)]
        static public void Go(string[] args, IProgressBar progress)
        {
            var infile = args[0];
            var outfile = args[1];
            var NBT = ITask.ReadNBT(infile, progress);
            var ofstream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            ofstream.Write(NBT.ToString()!);

            return;
        }
    }
    [WorldpackExtension]
    struct ParseAndWrite
    {
        [WorldpackCommand(Name = "parsedump", Help = """
        Parses a NBT file and writes a copy from its parsed form
        parsedump is an idempotent operation, if not, file a bug
        *    parsedump(parsedump(f)) = parsedump(f)
        """)]
        static public void Go(string[] args, IProgressBar progress)
        {
            var infile = args[0];
            var outfile = args[1];
            var NBT = ITask.ReadNBT(infile, progress);
            var ofstream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
            ArrayBufferWriter<byte> w = new();
            NBT.WriteTo(w);
            ofstream.Write([10, 0, 0]);// Unnamed Compound
            ofstream.Write(w.WrittenSpan);
        }
    }
}