using ShellProgressBar;
using Worldpack.Util;

namespace Worldpack.Tasks
{
    interface ITask
    {
        static NBT.CompoundTag ReadNBT(string infile, IProgressBar? progress = null)
        {
            var ifstream = new FileStream(infile, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[ifstream.Length];
            ifstream.ReadExactly(buffer);
            if (Identify.LooksLikeRegion(buffer))
            {
                var region = new RegionFile(buffer);
                return region.GetCompoundFormat(progress);
            }
            buffer = Identify.ConvertToUncompressed(buffer);
            return (NBT.CompoundTag)NBT.CompoundTag.Parse(new ArraySegment<byte>(buffer), out _);
        }
        
    }
}