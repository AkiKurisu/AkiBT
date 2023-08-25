using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Decorator : Execute the child node repeatedly by the specified number of times" +
     ", if the execution returns Failure, the loop ends and returns Failure")]
    [AkiLabel("Repeater")]
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
            for (int i = lastCount; i < repeatCount.Value; i++)
            {
                var status = Child.Update();
                if (status == Status.Success) continue;
                if (status == Status.Running)
                {
                    lastCount = i;
                }
                return status;
            }
            return Status.Success;
        }
        public override void Abort()
        {
            lastCount = 0;
        }
    }
}