using UnityEngine;
using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:获取Transform.Position")]
    [AkiLabel("Transform:GetPosition")]
    [AkiGroup("Transform")]
    public class TransformGetPosition : Action
    {
        [SerializeField,Tooltip("目标Transform,如不填写则为自身Transform")]
        private Transform target;
        [SerializeField,ForceShared,FormerlySerializedAs("result")]
        private SharedVector3 storeResult;
        public override void Awake()
        {
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            if(target!=null)storeResult.Value=target.position;
            else storeResult.Value=gameObject.transform.position;
            return Status.Success;
        }
    }
}