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
        private SharedVector3 result=new SharedVector3();//共享变量是一个类,需要new()一个实例
        public override void Awake()
        {
            InitVariable(result);//共享变量需要初始化
        }
        protected override Status OnUpdate()
        {
            if(target!=null)result.Value=target.transform.position;
            return Status.Success;
        }
    }
}