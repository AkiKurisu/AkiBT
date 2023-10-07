using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Get Transform.Position")]
    [AkiLabel("Transform : GetPosition")]
    [AkiGroup("Transform")]
    public class TransformGetPosition : Action
    {
        [SerializeField, Tooltip("Target Transform, if not filled in, it will be its own Transform")]
        private SharedTObject<Transform> target;
        [SerializeField, ForceShared]
        private SharedVector3 storeResult;
        public override void Awake()
        {
            InitVariable(storeResult);
            InitVariable(target);
        }
        protected override Status OnUpdate()
        {
            if (target.Value != null) storeResult.Value = target.Value.position;
            else storeResult.Value = GameObject.transform.position;
            return Status.Success;
        }
    }
}