using UnityEngine;
namespace Kurisu.AkiBT.Example
{
    [AkiInfo("Action:获取GameObject.transform.position")]
    [AkiLabel("Example:获取物体位置")]
    [AkiGroup("Example")]
    public class GetGameObjectPosition : Action
    {
        [SerializeField,AkiLabel("获取对象")]
        private GameObject target;
        [SerializeField,Tooltip("获取的位置会存储在该共享变量中")]
        private SharedVector3 result;//1.2.3版本没有new()实例不会在编辑器报错,而在之前的版本要在编辑器内显示必须要在脚本中new实例。
        public override void Awake()
        {
            InitVariable(result);//共享变量需要初始化才能实际绑定到黑板中的变量
        }
        protected override Status OnUpdate()
        {
            if(target!=null)result.Value=target.transform.position;
            return Status.Success;
        }
    }
}