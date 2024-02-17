using UnityEngine;
namespace Kurisu.AkiBT.Extend.Transform
{
    [AkiInfo("Action: Convert local position to world position")]
    [AkiLabel("Transform: LocalToWorldPosition")]
    [AkiGroup("Transform")]
    public class LocalToWorldPosition : Action
    {
        [Tooltip("Target transform, if not filled in, it will use owner's Transform")]
        public SharedTObject<UnityEngine.Transform> target;
        public SharedVector3 localPosition;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            if (target.Value != null) storeResult.Value = target.Value.TransformPoint(localPosition.Value);
            else storeResult.Value = GameObject.transform.TransformPoint(localPosition.Value);
            return Status.Success;
        }
    }
}