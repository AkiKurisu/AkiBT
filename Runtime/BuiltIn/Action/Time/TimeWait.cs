using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:等待一段时间")]
    [AkiLabel("Time:Wait")]
    [AkiGroup("Time")]
    public class TimeWait : Action
    {
        [SerializeField]
        private float waitTime;
        private float timer;     
        protected override Status OnUpdate()
        {
            timer+=Time.deltaTime;
            if(timer>=waitTime)
            {
                timer=0;
                return Status.Success;
            }  
            else
                return Status.Running;
        }
        public override void Abort()
        {
            timer=0;
        }
    }
}