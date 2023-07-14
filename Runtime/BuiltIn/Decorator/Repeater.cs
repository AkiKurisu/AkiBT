using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Repeater:按指定次数重复执行子结点,如果执行返回Failure则结束循环并返回Failure")]
    [AkiLabel("Repeater重复")]
    public class Repeater : Decorator
    {
        [SerializeField]
        private SharedInt repeatCount;
        private int lastCount;
        protected override void OnAwake()
        {
            InitVariable(repeatCount);
        }
        protected override Status OnUpdate()
        {
            for(int i=lastCount;i<repeatCount.Value;i++)
            {
                var status=Child.Update();
                if(status==Status.Success)continue;
                if(status==Status.Running)
                {
                    lastCount=i;
                }
                return status;
            }
            return Status.Success;
        }
        public override void Abort()
        {
            lastCount=0;
        }
    }
}