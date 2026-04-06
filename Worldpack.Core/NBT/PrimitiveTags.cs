using System.Buffers;
using Worldpack.Util;
namespace Worldpack.NBT
{
    public class ByteTag : ITag<sbyte>
    {
        public static ByteTag Of(byte b) => ALL[b];
        public static ByteTag Of(sbyte b) => ALL[(byte)b];
        static ByteTag[] ALL = new ByteTag[256];
        static ByteTag()
        {
            for (int i = 0; i < 256; i++) ALL[i] = new ByteTag((byte)i);
        }
        private ByteTag(byte v)
        {
            Value = (sbyte)v;
        }
        public sbyte Value { get; private set; }
        public Tag Type => Tag.Byte;
        public static ITag<sbyte> Parse(ArraySegment<byte> data, out int size)
        {
            size = 1;
            return ALL[data[0]];
        }
        public override string ToString() => $"{Value}b";

        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write([(byte)Value]);
        }
        public static implicit operator ByteTag(byte v) => ALL[v];
        public static implicit operator ByteTag(sbyte v) => ALL[(byte)v];

        public int Sizeof() => 1;
    }
    public class ShortTag : IMutableTag<short>
    {
        public short Value { get; private set; }

        public Tag Type => Tag.Short;
        public static ITag<short> Parse(ArraySegment<byte> data, out int size)
        {
            size = 2;
            return new ShortTag()
            {
                Value = BitConverter.ToInt16(data).FromBigEndian()
            };

        }
        public void SetValue(short val) => Value = val;
        public override string ToString() => $"{Value}s";

        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.ToBigEndian()));
        }
        public static implicit operator ShortTag(ushort v) => new() { Value = (short)v };
        public static implicit operator ShortTag(short v) => new() { Value = v };
        public int Sizeof() => 2;
    }

    public class IntTag : IMutableTag<int>
    {
        public int Value { get; private set; }

        public Tag Type => Tag.Int;
        public static ITag<int> Parse(ArraySegment<byte> data, out int size)
        {
            size = 4;
            return new IntTag() { Value = BitConverter.ToInt32(data).FromBigEndian() };
        }
        public override string ToString() => $"{Value}i";
        public void SetValue(int val) => Value = val;
        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.ToBigEndian()));
        }
        public int Sizeof() => 4;
                public static implicit operator IntTag(uint v) => new() { Value = (int)v };
        public static implicit operator IntTag(int v) => new() { Value = v };
    }

    public class LongTag : IMutableTag<long>
    {
        public long Value { get; private set; }
        public Tag Type => Tag.Long;
        public static ITag<long> Parse(ArraySegment<byte> data, out int size)
        {
            size = 8;
            return new LongTag() { Value = BitConverter.ToInt64(data).FromBigEndian() };
        }

        public void SetValue(long val) => Value = val;
        public override string ToString() => $"{Value}l";
        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.ToBigEndian()));
        }
        public int Sizeof() => 8;
                public static implicit operator LongTag(ulong v) => new() { Value = (long)v };
        public static implicit operator LongTag(long v) => new() { Value = v };
    }

    public class FloatTag : IMutableTag<float>
    {
        public float Value { get; private set; }

        public Tag Type => Tag.Float;
        public static ITag<float> Parse(ArraySegment<byte> data, out int size)
        {
            size = 4;
            return new FloatTag()
            {
                Value = BitConverter.ToSingle(data).FromBigEndian()
            };
        }
        public override string ToString() => $"{Value}f";
        public void SetValue(float val) => Value = val;
        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.ToBigEndian()));
        }
        public static implicit operator FloatTag(float v) => new() { Value = v };
        public int Sizeof() => 4;
    }

    public class DoubleTag : IMutableTag<double>
    {
        public double Value { get; private set; }

        public Tag Type => Tag.Double;
        public static ITag<double> Parse(ArraySegment<byte> data, out int size)
        {
            size = 8;
            return new DoubleTag()
            {
                Value = BitConverter.ToDouble(data).FromBigEndian()
            };
        }
        public override string ToString() => $"{Value}d";
        public void SetValue(double val) => Value = val;
        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write(BitConverter.GetBytes(Value.ToBigEndian()));
        }
        public int Sizeof() => 8;
        public static implicit operator DoubleTag(double v) => new() { Value = v };
    }
}