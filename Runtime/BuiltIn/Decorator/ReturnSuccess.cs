namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Decorator : Regardless of the return value of the child node, always return Success")]
    public class ReturnSuccess : Decorator
    {
        protected override Status OnDecorate(Status childStatus)
        {
            return Status.Success;
        }
    }
}