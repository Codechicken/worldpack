using System.Buffers;
using System.Text;
namespace Worldpack.Util
{
    public static class ArrayExtensions
    {
        public static string ReadJString(this ArraySegment<byte> data, out int size)
        {
            int len = BitConverter.ToUInt16(data).FromBigEndian();
            size = len + 2;
            var buf = data.Slice(2, len);
            return string.Intern(new(Encoding.UTF8.GetChars([.. buf])));
        }
    }
    public static class StringExtensions
    {
        public static void WriteJString(this string s, IBufferWriter<byte> writer)
        {
            var buf = Encoding.UTF8.GetBytes(s);
            ushort sz = checked((ushort)buf.Length); // Explode if too big
            writer.Write(BitConverter.GetBytes(sz.ToBigEndian()));
            writer.Write(buf);
        }
    }
    public static class EndianessExtensions
    {
        //for completeness 
        public static byte SwapEndian(this byte val) => val;
        public static sbyte SwapEndian(this sbyte val) => val;
        public static byte ToBigEndian(this byte val) => val;
        public static byte FromBigEndian(this byte val) => val;
        public static sbyte ToBigEndian(this sbyte val) => val;
        public static sbyte FromBigEndian(this sbyte val) => val;

        public static ushort ToBigEndian(this ushort val) => BitConverter.IsLittleEndian ? val.SwapEndian() : val;
        public static ushort FromBigEndian(this ushort val) => !BitConverter.IsLittleEndian ? val : val.SwapEndian();
        public static short ToBigEndian(this short val) => (short)((ushort)val).ToBigEndian();
        public static short FromBigEndian(this short val) => (short)((ushort)val).FromBigEndian();
        public static ushort SwapEndian(this ushort val) => (ushort)((val & 0xff_00) >> 8 | (val & 0xff) << 8);
        public static short SwapEndian(this short val) => (short)((ushort)val).SwapEndian();

        public static uint ToBigEndian(this uint val) => BitConverter.IsLittleEndian ? val.SwapEndian() : val;
        public static uint FromBigEndian(this uint val) => !BitConverter.IsLittleEndian ? val : val.SwapEndian();
        public static int ToBigEndian(this int val) => (int)((uint)val).ToBigEndian();
        public static int FromBigEndian(this int val) => (int)((uint)val).FromBigEndian();
        public static uint SwapEndian(this uint val) => (val & 0xff_00_00_00) >> 24 | (val & 0x00_ff_00_00) >> 8 | (val & 0x00_00_ff_00) << 8 | (val & 0x00_00_00_ff) << 24;
        public static int SwapEndian(this int val) => (int)((uint)val).SwapEndian();

        public static ulong ToBigEndian(this ulong val) => BitConverter.IsLittleEndian ? val.SwapEndian() : val;
        public static ulong FromBigEndian(this ulong val) => !BitConverter.IsLittleEndian ? val : val.SwapEndian();
        public static long ToBigEndian(this long val) => (long)((ulong)val).ToBigEndian();
        public static long FromBigEndian(this long val) => (long)((ulong)val).FromBigEndian();
        public static ulong SwapEndian(this ulong val) =>
        (val & 0xff_00_00_00_00_00_00_00) >> 56
        | (val & 0x00_ff_00_00_00_00_00_00) >> 40
        | (val & 0x00_00_ff_00_00_00_00_00) >> 24
        | (val & 0x00_00_00_ff_00_00_00_00) >> 8
        | (val & 0x00_00_00_00_ff_00_00_00) << 8
        | (val & 0x00_00_00_00_00_ff_00_00) << 24
        | (val & 0x00_00_00_00_00_00_ff_00) << 40
        | (val & 0x00_00_00_00_00_00_00_ff) << 56
        ;
        public static long SwapEndian(this long val) => (long)((ulong)val).SwapEndian();

        public static float ToBigEndian(this float val) => BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(val).ToBigEndian());
        public static float FromBigEndian(this float val) => BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(val).FromBigEndian());
        public static float SwapEndian(this float val) => BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(val).SwapEndian());
        public static double ToBigEndian(this double val) => BitConverter.UInt64BitsToDouble(BitConverter.DoubleToUInt64Bits(val).ToBigEndian());
        public static double FromBigEndian(this double val) => BitConverter.UInt64BitsToDouble(BitConverter.DoubleToUInt64Bits(val).FromBigEndian());
        public static double SwapEndian(this double val) => BitConverter.UInt64BitsToDouble(BitConverter.DoubleToUInt64Bits(val).SwapEndian());
    }
}