namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Decorator : If the child node returns Success, it is reversed to Failure," +
   " if it is Failure, it is reversed to Success.")]
    [AkiLabel("Invertor")]
    public class Invertor : Decorator
    {
        protected override Status OnDecorate(Status childStatus)
        {
            if (childStatus == Status.Success)
                return Status.Failure;
            else if (childStatus == Status.Failure)
                return Status.Success;
            else
                return childStatus;
        }
        protected override bool OnDecorate(bool childCanUpdate)
        {
            return !childCanUpdate;
        }
    }
}