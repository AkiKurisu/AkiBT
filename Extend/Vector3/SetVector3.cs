namespace Kurisu.AkiBT.Extend.Vector3
{
    [AkiInfo("Action: Set Vector3 value")]
    [AkiLabel("Vector3: SetVector3")]
    [AkiGroup("Vector3")]
    public class SetVector3 : Action
    {
        public SharedVector3 vector3;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = vector3.Value;
            return Status.Success;
        }
    }
}
