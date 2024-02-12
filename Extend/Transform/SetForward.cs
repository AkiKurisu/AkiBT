using UnityEngine;
namespace Kurisu.AkiBT.Extend.Transform
{
    [AkiInfo("Action: Set transform forward Vector3")]
    [AkiLabel("Transform: SetForward")]
    [AkiGroup("Transform")]
    public class SetForward : Action
    {
        [Tooltip("Target transform, if not filled in, it will use owner's Transform")]
        public SharedTObject<UnityEngine.Transform> target;
        public SharedVector3 position;
        protected override Status OnUpdate()
        {
            UnityEngine.Transform targetTransform = target.Value != null ? target.Value : GameObject.transform;
            targetTransform.forward = position.Value;
            return Status.Success;
        }
    }
}