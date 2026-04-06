using ShellProgressBar;
using Worldpack.NBT;
namespace Worldpack.Util
{
    public class RegionFile
    {
        int[,] times = new int[32, 32];
        byte[,][] data = new byte[32, 32][];
        public RegionFile(byte[] data)
        {
            if (!Identify.LooksLikeRegion(data)) throw new InvalidDataException("Not a region file");
            for (int z = 0; z < 32; z++) for (int x = 0; x < 32; x++)
                {
                    uint record = BitConverter.ToUInt32(data.AsSpan()[(128 * z + 4 * x)..]).FromBigEndian();
                    times[z, x] = BitConverter.ToInt32(data.AsSpan()[(0x1000 + 128 * z + 4 * x)..]).FromBigEndian();
                    var offset = record >> 8 << 12;
                    var size = record & 0xff;
                    if (record == 0)
                    {
                        this.data[z, x] = [];
                        continue;
                    }
                    var len = BitConverter.ToUInt32(data.AsSpan()[(int)offset..]).FromBigEndian() - 1;
                    offset += 5;
                    this.data[z, x] = data[(int)offset..(int)(offset + len)];

                }
        }
        public static explicit operator RegionFile (FileStream ifstream)
        {
            byte[] buffer = new byte[ifstream.Length];
            ifstream.ReadExactly(buffer);
            return new(buffer);
        }
        public CompoundTag this[int X, int Z]
        {
            get
            {
                byte[] buf = data[Z, X];
                if (buf.Length == 0) return new CompoundTag(); // no chunk
                return (CompoundTag)CompoundTag.Parse(new ArraySegment<byte>(Identify.ConvertToUncompressed(buf)).Slice(3), out _);
            }
        }
        public CompoundTag GetCompoundFormat(IProgressBar? progress)
        {

            const string Alphabet = "abcdefghijklmnopqrstuvwxyz012345";
            var tag = new CompoundTag();
            tag["worldpack:region"] = ByteTag.Of(1);
            IntArrayTag times = new();
            times.SetValue(new int[1024]);
            tag["worldpack:region_timestamps"] = times;
            ChildProgressBar? task = null;
            if (progress!=null)
            {
                task = progress.Spawn(1024, "Reading region");
            }
            for (int x = 0; x < 32; x++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        tag[Alphabet[x].ToString() + Alphabet[z].ToString()] = this[x, z];
                        task?.Tick();
                        times.Value[z * 32 + x] = this.times[z, x];
                    }
                }
            return tag;
        }
    }
}
/**
Internal region format
Compound{
    [base32][base32] : Compound [Chunk]
    worldpack:region : Byte = 1
    worldpack:region_timestamps : IntArray[1024]
}
*/