using System.IO.Compression;

namespace Worldpack.Util
{
    public enum FormatType
    {
        Region,
        Zlib,
        GZip,
        Uncompressed,
        Unknown,
    }
    public static class Identify
    {
        public static bool LooksLikeRegion(ReadOnlySpan<byte> buf)
        {
            if ((buf.Length & 0xfff) != 0) return false;
            var pages = (uint)buf.Length >> 12;
            if (pages < 2) return false;
            var page0 = buf[..0x1000];
            for (int i = 0; i < 32 * 32; i++)
            {
                uint record = BitConverter.ToUInt32(page0[(4 * i)..]).FromBigEndian();
                var offset = record >> 8;
                if (offset == 0) continue;
                if (offset == 1) return false;
                var len = record & 0xff;
                if (offset + len > pages) return false;
                var bytesize = BitConverter.ToUInt32(buf[(int)(offset << 12)..]).FromBigEndian() + 4;//data length does not include itself
                var lenbytes = len << 12;
                if (lenbytes < bytesize || bytesize + 0x1000<lenbytes) return false;
            }

            return true;
        }
        public static bool LooksLikeGZipNBT(byte[] data)
        {
            Span<byte> header = stackalloc byte[3];
            if (data[0] != 0x1f || data[1] != 0x8b) return false;
            try
            {
                new GZipStream(new MemoryStream(data), CompressionMode.Decompress).ReadAtLeast(header, 3);
            }
            catch
            {
                return false;
            }
            return LooksLikeUncompressedNBT(header);
        }

        public static FormatType SimpleIdentify(byte[] data)
        {
            if (LooksLikeRegion(data)) return FormatType.Region;
            if (LooksLikeGZipNBT(data)) return FormatType.GZip;
            if (LooksLikeZlibNBT(data)) return FormatType.Zlib;
            if (LooksLikeUncompressedNBT(data)) return FormatType.Uncompressed;

            return FormatType.Unknown;
        }

        public static bool LooksLikeZlibNBT(byte[] data)
        {
            if ((data[0] & 0xf) != 8) return false;
            if ((data[0] & 0xf0) >= 0x80) return false;
            if (((data[0] << 8 | data[1]) % 31) != 0) return false;
            Span<byte> header = stackalloc byte[3];
            try
            {
                new ZLibStream(new MemoryStream(data), CompressionMode.Decompress).ReadAtLeast(header, 3);
            }
            catch
            {
                return false;
            }
            return LooksLikeUncompressedNBT(header);
        }

        public static bool LooksLikeUncompressedNBT(ReadOnlySpan<byte> header)
        {
            if (header[0] != 10 && header[0] != 9) return false;
            return header[1] == 0 && header[2] == 0;
        }
        public static byte[] ConvertToUncompressed(byte[] data) => SimpleIdentify(data) switch
        {
            FormatType.Uncompressed => data,
            FormatType.Zlib => new ZLibStream(new MemoryStream(data), CompressionMode.Decompress).ReadAll(),
            FormatType.GZip => new GZipStream(new MemoryStream(data), CompressionMode.Decompress).ReadAll(),
            _ => throw new ArgumentException("Not a compressed NBT buffer"),
        };
    }
}