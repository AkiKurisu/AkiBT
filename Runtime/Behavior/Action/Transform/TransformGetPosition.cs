using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Get Transform.Position")]
    [AkiLabel("Transform: GetPosition")]
    [AkiGroup("Transform")]
    public class TransformGetPosition : Action
    {
        [Tooltip("Target Transform, if not filled in, it will be its own Transform")]
        public SharedTObject<Transform> target;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            if (target.Value) storeResult.Value = target.Value.position;
            else storeResult.Value = GameObject.transform.position;
            return Status.Success;
        }
    }
}