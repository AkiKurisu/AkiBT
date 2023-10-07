using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Timer, wait for a period of time, return to Running during the period," +
    " return to Success at the end, and restore the count after Abort interrupts")]
    [AkiLabel("Time :  Wait")]
    public class TimeWait : Action
    {
        [SerializeField]
        private SharedFloat waitTime;
        private float timer;
        public override void Awake()
        {
            InitVariable(waitTime);
        }
        protected override Status OnUpdate()
        {
            AddTimer();
            if (IsAlready())
            {
                ClearTimer();
                return Status.Success;
            }
            else
                return Status.Running;
        }
        private void AddTimer()
        {
            timer += Time.deltaTime;
        }
        private void ClearTimer()
        {
            timer = 0;
        }
        private bool IsAlready() => timer > waitTime.Value;
        public override void Abort()
        {
            ClearTimer();
        }
    }
}