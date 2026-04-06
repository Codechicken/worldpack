using Worldpack.NBT;
using Worldpack.Util;

namespace Worldpack.Analysis
{
    [Analyzer(Priority = 0, Aggregator = typeof(Biome2DAggregator), Scale = AnalyzerAttribute.ProcessScale.Region)]
    class BiomeAnalyzer2D
    {
        byte[] biomes = new byte[512 * 512]; //Z,X
        public void Analyze(RegionFile region)
        {
            for (int z = 0; z < 32; z++) for (int x = 0; x < 32; x++)
                {
                    var chunk = region[x, z];
                    if (chunk["Level", "Biomes"] is ByteArrayTag b)
                    {
                        int bz = z << 4, bx = x << 4;
                        for (int iz = 0; iz < 16; iz++) Array.Copy(b.Value, iz << 4, biomes, (bz + iz) * 512 + bx, 16);
                    }
                    else
                    {
                        int bz = z << 4, bx = x << 4;
                        for (int iz = 0; iz < 16; iz++) Array.Fill<byte>(biomes, 255, (bz + iz) * 512 + bx, 16);
                    }

                }
        }
        public byte[] Value{ get => biomes; }
    }
    
    class Biome2DAggregator
    {

    }
}