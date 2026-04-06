using System.Buffers;
using System.Text;
using Worldpack.Util;
namespace Worldpack.NBT
{
    public class ByteArrayTag : IMutableTag<byte[]>
    {
        public byte[] Value { get; private set; } = [];
        public Tag Type => Tag.ByteArray;
        public static ITag<byte[]> Parse(ArraySegment<byte> data, out int size)
        {
            ByteArrayTag ret = new();
            size = 4;
            var count = BitConverter.ToInt32(data).FromBigEndian();
            data = data.Slice(4);
            size += count;
            var buf = data.Slice(0,count);
            ret.Value = [.. buf];
            return ret;
        }
        public override string ToString()
        {
            StringBuilder buf = new();
            buf.Append('[');
            for (var i = 0; i < Value.Length; i++)
            {
                if ((i & 0xf) == 0)
                {
                    buf.Append('\n');
                }
                buf.Append($"{Value[i]}, ");
            }
            buf.Append("\n");
            return buf.ToString();
        }
        public void SetValue(byte[] val) => Value = val;

        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.Length.ToBigEndian()));
            writer.Write(Value);
        }
        public static implicit operator ByteArrayTag(byte[] v) => new() { Value = v };
        public int Sizeof() => Value.Length+4;
    }
    public class StringTag(string v) : ITag<string>
    {
        static Dictionary<string, StringTag> intern = [];
        public string Value { get; private set; } = v;

        public Tag Type => Tag.String;
        public static ITag<string> Parse(ArraySegment<byte> data, out int size)
        {
            var s = data.ReadJString(out size);
            if (intern.TryGetValue(s, out var tag)) return tag;
            StringTag ret = new(s);
            intern[s] = ret;
            return ret;
        }
        public override string ToString() => $"\"{Value}\"";
        public void WriteTo(IBufferWriter<byte> writer)
        {
            Value.WriteJString(writer);
        }
        public static implicit operator StringTag(string s)
        {
            if (intern.TryGetValue(s, out var tag)) return tag;
            StringTag ret = new(s);
            intern[s] = ret;
            return ret;
        }
        public int Sizeof() => 2 + (ushort)Encoding.UTF8.GetByteCount(Value);
    }
    public class IntArrayTag : IMutableTag<int[]>
    {
        public int[] Value { get; private set; } = [];
        public Tag Type => Tag.IntArray;
        public static ITag<int[]> Parse(ArraySegment<byte> data, out int size)
        {
            var count = BitConverter.ToInt32(data).FromBigEndian();
            IntArrayTag ret = new();
            size = 4 + 4 * count;
            var d = data.Slice(4, 4 * count).ToArray();
            ret.Value = new int[count];
            for (var i = 0; i < count; i++)
            {
                ret.Value[i] = BitConverter.ToInt32(d, i * 4).FromBigEndian();
            }
            return ret;
        }
        public override string ToString()
        {
            StringBuilder buf = new();
            buf.Append('[');
            for (var i = 0; i < Value.Length; i++)
            {
                if ((i & 0x3) == 0)
                {
                    buf.Append('\n');
                }
                buf.Append($"{Value[i]}, ");
            }
            buf.Append("]\n");
            return buf.ToString();
        }
        public void SetValue(int[] val) => Value = val;

        public void WriteTo(IBufferWriter<byte> writer)
        {
            Span<byte> buffer = stackalloc byte[4];
            BitConverter.TryWriteBytes(buffer, Value.Length.ToBigEndian());
            writer.Write(buffer);
            for (int i = 0; i < Value.Length; i++)
            {
                BitConverter.TryWriteBytes(buffer, Value[i].ToBigEndian());
                writer.Write(buffer);
            }
        }
        public static implicit operator IntArrayTag(int[] v) => new() { Value = v };
        public int Sizeof() => 4 * Value.Length+4;
    }
    public class LongArrayTag : IMutableTag<long[]>
    {
        public long[] Value { get; private set; } = [];
        public Tag Type => Tag.LongArray;
        public static ITag<long[]> Parse(ArraySegment<byte> data, out int size)
        {
            LongArrayTag ret = new();

            var count = BitConverter.ToInt32(data).FromBigEndian();
            size = 4 + 8 * count;
            var d = data.Slice(4, 8 * count).ToArray();
            ret.Value = new long[count];
            for (var i = 0; i < count; i++)
            {
                ret.Value[i] = BitConverter.ToInt64(d, i * 8).FromBigEndian();
            }
            return ret;
        }
        public override string ToString()
        {
            StringBuilder buf = new();
            buf.Append('[');
            for (var i = 0; i < Value.Length; i++)
            {
                if ((i & 0x3) == 0x0)
                {
                    buf.Append('\n');
                }
                buf.Append($"{Value[i]}, ");
            }
            buf.Append("]\n");
            return buf.ToString();
        }
        public void SetValue(long[] val) => Value = val;
        public void WriteTo(IBufferWriter<byte> writer)
        {
            Span<byte> buffer = stackalloc byte[8];
            BitConverter.TryWriteBytes(buffer, Value.Length.ToBigEndian());
            writer.Write(buffer[0..4]);
            for (int i = 0; i < Value.Length; i++)
            {
                BitConverter.TryWriteBytes(buffer, Value[i].ToBigEndian());
                writer.Write(buffer);
            }
        }
        public int Sizeof() => 8 * Value.Length + 4;
        public static implicit operator LongArrayTag(long[] v) => new() { Value = v };
    }
}