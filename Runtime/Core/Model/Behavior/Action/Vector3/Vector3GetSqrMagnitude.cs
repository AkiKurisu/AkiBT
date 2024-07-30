namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Calculate the square of the Vector3 modulus, " +
    "the performance is better than Distance, but the accuracy will be lost")]
    [AkiLabel("Vector3: GetSqrMagnitude")]
    [AkiGroup("Vector3")]
    public class Vector3GetSqrMagnitude : Action
    {
        public SharedVector3 vector3;
        [ForceShared]
        public SharedFloat result;
        protected override Status OnUpdate()
        {
            result.Value = vector3.Value.sqrMagnitude;
            return Status.Success;
        }
    }
}
