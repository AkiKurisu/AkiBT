using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition:计数器,每次增加Time.deltaTime,满足条件后返回Success")]
    [AkiLabel("Time:TimerCondition")]
    [AkiGroup("Time")]
public class TimerCondition : Conditional
{
    [SerializeField]
    private float waitTime;
    private float timer;
     protected override bool IsUpdatable()
    {
        timer+=Time.deltaTime;
        if(timer>=waitTime)
        {
            timer=0;
            return true;
        }
        return false;
    }
     public override void Abort()
    {
        base.Abort();
        timer=0;
    }
}
}
