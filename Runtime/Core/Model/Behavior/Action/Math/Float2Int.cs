namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Convert float type to int type")]
    [AkiLabel("Math: Float2Int")]
    [AkiGroup("Math")]
    public class Float2Int : Action
    {
        public SharedFloat value;
        [ForceShared]
        public SharedInt storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = (int)value.Value;
            return Status.Success;
        }
    }
}
