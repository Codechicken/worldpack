namespace Worldpack.Analysis
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AnalyzerAttribute : Attribute
    {
        public required int Priority;
        public enum ProcessScale
        {
            Chunk = 0,
            File = Chunk,
            Region = 1,
            Dimension = 2,
        }
        public required ProcessScale Scale;
        public Type? Aggregator;
        public bool Reusable = false;
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AggregatorAttribute : Attribute
    {
        public required Type Result;

    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class GlobalAnayzerAttribute : Attribute
    {
        public required int Priority;
        public required Type Result;
    }
}