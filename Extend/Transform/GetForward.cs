using UnityEngine;
namespace Kurisu.AkiBT.Extend.Transform
{
    [AkiInfo("Action: Get transform forward Vector3")]
    [AkiLabel("Transform: GetForward")]
    [AkiGroup("Transform")]
    public class GetForward : Action
    {
        [Tooltip("Target transform, if not filled in, it will use owner's Transform")]
        public SharedTObject<UnityEngine.Transform> target;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            UnityEngine.Transform targetTransform = target.Value != null ? target.Value : GameObject.transform;
            storeResult.Value = targetTransform.forward;
            return Status.Success;
        }
    }
}