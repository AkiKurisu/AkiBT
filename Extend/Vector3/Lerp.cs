using UnityEngine;
namespace Kurisu.AkiBT.Extend.Vector3
{
    [AkiInfo("Action: Lerp Vector3")]
    [AkiLabel("Vector3: Lerp")]
    [AkiGroup("Vector3")]
    public class Lerp : Action
    {
        [Tooltip("The from value")]
        public SharedVector3 fromVector3;
        [Tooltip("The to value")]
        public SharedVector3 toVector3;
        [Tooltip("The amount to lerp")]
        public SharedFloat lerpAmount;
        [Tooltip("The lerp result"), ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = UnityEngine.Vector3.Lerp(fromVector3.Value, toVector3.Value, lerpAmount.Value);
            return Status.Success;
        }
    }
}
