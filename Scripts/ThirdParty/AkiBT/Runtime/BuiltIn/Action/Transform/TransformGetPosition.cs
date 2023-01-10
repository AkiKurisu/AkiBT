using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiInfo("Action:获取Transform.Position")]
    [AkiLabel("Transform:GetPosition")]
    [AkiGroup("Transform")]
    public class TransformGetPosition : Action
    {
        [SerializeField,Tooltip("目标Transform")]
        private Transform target;
        [SerializeField]
        private SharedVector3 result;
        public override void Awake()
        {
            InitVariable(result);
        }
        protected override Status OnUpdate()
        {
            if(target!=null)result.Value=target.position;
            return Status.Success;
        }
    }
}