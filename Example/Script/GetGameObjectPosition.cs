using UnityEngine;
namespace Kurisu.AkiBT.Example
{
    [AkiInfo("Action:获取GameObject.transform.position")]
    [AkiLabel("Example:获取物体位置")]
    [AkiGroup("Example")]
    public class GetGameObjectPosition : Action
    {
        [SerializeField, Tooltip("获取对象")]
        private SharedTObject<GameObject> target;
        [SerializeField, Tooltip("获取的位置会存储在该共享变量中")]
        private SharedVector3 result;
        public override void Awake()
        {
            InitVariable(target);
            InitVariable(result);
        }
        protected override Status OnUpdate()
        {
            if (target.Value != null) result.Value = target.Value.transform.position;
            return Status.Success;
        }
    }
}