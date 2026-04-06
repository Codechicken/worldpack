using System.Reflection;
namespace Worldpack.Analysis
{
    public class AnalysisControl
    {
        void DiscoverAnalyzers()
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()){
                if (ass.GetCustomAttribute<WorldpackExtensionAttribute>()!= null)
                {
                    
                }
            }
        }
    }
}