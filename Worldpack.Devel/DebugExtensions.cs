using Worldpack;
using Worldpack.Util;
[assembly: WorldpackExtension]
namespace Worldpack.Devel;
 [WorldpackExtension]
    class Dev_debugExtensions
    {
        [WorldpackCommand(Name = "devel_incorrect_extension")]
        internal static void Go(string[] _args, ShellProgressBar.IProgressBar _bar)
        {
            CommandDiscoverer.DiscoverInvalid();
        }
    }