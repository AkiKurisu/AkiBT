namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Decorator:无论子结点返回值,始终返回Success")]
    [AkiLabel("ReturnSuccess返回成功")]
public class ReturnSuccess : Decorator
{
    protected override Status OnDecorate(Status childStatus)
    {
        return Status.Success;
    }
}
}