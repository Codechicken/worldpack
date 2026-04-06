
using ShellProgressBar;
using Worldpack.Util;
namespace Worldpack.Core.Tasks
{
    [WorldpackExtension]
    class Help
    {
        [WorldpackCommand(Name = "help", Help = "print Help for command")]
        public static void Go(string[] args, IProgressBar _bar)
        {
            if (args.Length != 0)
            {
                var cmds = CommandDiscoverer.Discover();
                Console.WriteLine(cmds.First(e => e.Key.Name == args[0]).Key.Help);
            } else {
                foreach (var c in CommandDiscoverer.Discover()){
                    Console.Write($"{c.Key.Name}: {c.Key.Help}\n");

                }
            }
        }
    }
}
