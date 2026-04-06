namespace Worldpack.Util
{
    public enum DataVersion
    {
        Pre_DataVersions = 0,
        DataVersions_Intro = 100,
        Flattening = 1451,
        Biomes3D = 2203
    }
    public readonly struct RegionVersions
    {
        public DataVersion Min { get; }
        public DataVersion Max { get; }
    }
}