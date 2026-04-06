using Worldpack.Util;
using ShellProgressBar;
namespace Worldpack
{
    public class Worldpack
    {
        public static void Main(string[] args)
        {
            ExtensionDiscoverer.Discover();
            var Commands = CommandDiscoverer.Discover();
            if (args.Length < 1)
            {
                goto usage;

            }
            var progress = new ProgressBar(1, "Worldpack", new ProgressBarOptions
            {
                CollapseWhenFinished = true,
                ProgressCharacter = '#',
            });
            var Target = Commands.First((e) => e.Key.Name == args[0]);
            Target.Value(args[1..], progress);
            progress.Tick();
            return;
        usage:;
            Console.WriteLine("Available Commands:");
            foreach (var s in Commands.OrderBy((e) => e.Key.Name))
            {
                Console.WriteLine(s.Key.Name);
            }
        }

    }
}
