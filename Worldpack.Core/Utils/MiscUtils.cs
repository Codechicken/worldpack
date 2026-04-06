//#define MEM_USAGE
using System.Text;
using ShellProgressBar;
namespace Worldpack.Util
{
    static class StreamExtensions
    {
        public static byte[] ReadAll(this Stream s)
        {
            var acc = new byte[4096];
            byte[] collector = new byte[4096];
            int collectorPt = 0;
            int this_sz = 0;
            while ((this_sz = s.Read(acc)) != 0)
            {
                if (collector.Length - collectorPt < this_sz)
                {
                    byte[] newCollector = new byte[collector.Length * 2];
                    Array.Copy(collector, 0, newCollector, 0, collectorPt);
                    collector = newCollector;
                }
                Array.Copy(acc, 0, collector, collectorPt, this_sz);
                collectorPt += this_sz;
            }
            return collector[..collectorPt];
        }
        public static void Write(this FileStream f, string s) => f.Write(Encoding.UTF8.GetBytes(s));
    }
    static class ProgressExtensions
    {

        public static void ReportMem(this IProgressBar b)
        {
#if MEM_USAGE
            GC.Collect();
            var info = GC.GetGCMemoryInfo();
            b.WriteLine($"{info.HeapSizeBytes}");
#endif
        }
    }
}