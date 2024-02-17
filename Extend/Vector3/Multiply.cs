using UnityEngine;
namespace Kurisu.AkiBT.Extend.Vector3
{
    [AkiInfo("Action: Multiply Vector3")]
    [AkiLabel("Vector3: Multiply")]
    [AkiGroup("Vector3")]
    public class Multiply : Action
    {
        public SharedVector3 vector3;
        [Tooltip("The value to multiply the Vector3 of")]
        public SharedFloat multiplyBy;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = vector3.Value * multiplyBy.Value;
            return Status.Success;
        }
    }
}
