namespace Worldpack;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct)]
public class WorldpackExtensionAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Method)]
public class WorldpackCommandAttribute : Attribute
{
    public required string Name;
    public string Help = "";
}
