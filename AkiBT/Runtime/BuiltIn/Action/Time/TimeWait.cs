using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:计时器,等待一段时间,期间返回Running,结束返回Success,Abort打断后复原计数")]
    [AkiLabel("Time:Wait")]
    public class TimeWait : Action
    {
        [SerializeField]
        private float waitTime; 
        [SerializeField]
        private SharedFloat sharedTimer=new SharedFloat();  
        public override void Awake() {
            sharedTimer.GetValueFromTree<float>(tree);
 
        }
        protected override Status OnUpdate()
        {
            AddTimer();
            if(IsAlready())
            {
                ClearTimer();
                return Status.Success;
            }  
            else
                return Status.Running;
        }
        void AddTimer()
        {
             if(sharedTimer!=null)
                sharedTimer.Value+=Time.deltaTime;
        }
        void ClearTimer()
        {
             if(sharedTimer!=null)
                sharedTimer.Value=0;
        }
        bool IsAlready()
        {
            if(sharedTimer!=null)
                return sharedTimer.Value>waitTime;
            else
                return true;
        }
        public override void Abort()
        {
            ClearTimer();
        }
    }
}