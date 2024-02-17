namespace Kurisu.AkiBT.Extend.Vector3
{
    [AkiInfo("Action: Normalize Vector3")]
    [AkiLabel("Vector3: Normalize")]
    [AkiGroup("Vector3")]
    public class Normalize : Action
    {
        public SharedVector3 vector3;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = vector3.Value.normalized;
            return Status.Success;
        }
    }
}
