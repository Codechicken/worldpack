using ShellProgressBar;
using Worldpack.Analysis;
using Worldpack.Util;
namespace Worldpack.Tasks.Misc;
[WorldpackExtension]
class BiomeDrawer
{
    [WorldpackCommand(Name = "debug_drawBiomes2D", Help = "")]
    public static void Go(string[] args, IProgressBar _progress)
    {
        var infile = args[0];
        var outfile = args[1];
        var ifstream = new FileStream(infile, FileMode.Open, FileAccess.Read);
        var region = (RegionFile)ifstream;
        var ofstream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
        BiomeAnalyzer2D analyzer = new();
        analyzer.Analyze(region);
        DDSWriter.Write2DImage(analyzer.Value, 512, new BinaryWriter(ofstream));
    }
}