using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShellProgressBar;
namespace Worldpack.Util
{
    public class CommandDiscoverer
    {
        public delegate void WorldpackCommandAction(string[] args, IProgressBar bar);
        static readonly Dictionary<WorldpackCommandAttribute, WorldpackCommandAction> result = [];
        static bool Complete = false;
        [RequiresUnreferencedCode("")]
        public static Dictionary<WorldpackCommandAttribute, WorldpackCommandAction> Discover()
        {
            if (Complete) return result;

            foreach (var t in ExtensionDiscoverer.Discover())
            {
                foreach (var m in t.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new MemberFilter((i, _) => i.GetCustomAttribute<WorldpackCommandAttribute>() != null), null))
                {
                    try
                    {
                        result[m.GetCustomAttribute<WorldpackCommandAttribute>()!] = ((MethodInfo)m).CreateDelegate<WorldpackCommandAction>();
                    }
                    catch { }
                }
            }

            Complete = true;
            return result;
        }
        [RequiresUnreferencedCode("")]
        public static void DiscoverInvalid()
        {
            foreach (var t in ExtensionDiscoverer.Discover())
            {
                foreach (var m in t.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, new MemberFilter((i, _) => i.GetCustomAttribute<WorldpackCommandAttribute>() != null), null))
                {
                    var meth = (MethodInfo)m;
                    var nm = m.GetCustomAttribute<WorldpackCommandAttribute>()!.Name;
                    if (!meth.IsStatic) Console.WriteLine($"Command not static : {nm} : {meth.Name} in {meth.DeclaringType!.FullName}");
                    if (meth.GetParameters().Length != 2)
                    {
                        Console.WriteLine($"Command does not have 2 args : {nm} : {meth.Name} in {meth.DeclaringType!.FullName}");
                        continue;
                    }
                    if (meth.GetParameters()[0].ParameterType != typeof(string[]))
                    {
                        Console.WriteLine($"Command does not have arg0 as `string[]` : {nm} : {meth.Name} in {meth.DeclaringType!.FullName}");

                    }
                    if (meth.GetParameters()[1].ParameterType != typeof(IProgressBar))
                    {
                        Console.WriteLine($"Command does not have arg1 as `IProgressBar` : {nm} : {meth.Name} in {meth.DeclaringType!.FullName}");
                    }
                    if (meth.ReturnType != typeof(void))
                    {
                        Console.WriteLine($"Command does not return `void` : {nm} : {meth.Name} in {meth.DeclaringType!.FullName}");
                    }
                }
            }
        }
    }
    public class ExtensionDiscoverer
    {
        static bool DiscoverComplete = false;
        static readonly HashSet<Type> cache = [];
        [RequiresUnreferencedCode("")]
        public static HashSet<Type> Discover()
        {
            if (DiscoverComplete) return cache;
            string LaunchDir = typeof(ExtensionDiscoverer).Assembly.Location.Replace("Worldpack.API.dll",null);
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetCustomAttribute<WorldpackExtensionAttribute>() == null) continue;
                foreach (var t in  a.DefinedTypes)
                if (t.GetCustomAttribute<WorldpackExtensionAttribute>() != null)
                {
                    cache.Add(t);
                }
            }
            foreach (var s in Directory.EnumerateFiles(LaunchDir, "*.plug.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var a = Assembly.LoadFile(s);
                    if (a.GetCustomAttribute<WorldpackExtensionAttribute>() == null) //intentional side effect
                    {
                        Console.WriteLine($"Not a plugin: {s}");
                        continue;
                    }
                    foreach (var t in a.GetTypes())
                    {
                        if (t.GetCustomAttribute<WorldpackExtensionAttribute>() != null)
                        {
                            cache.Add(t);
                        }
                    }
                }
                catch
                {

                }
            }
            DiscoverComplete = true;
            return cache;
        }
    }
   
}