using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition:计时判断,每次增加Time.deltaTime,满足条件后返回True")]
    [AkiLabel("Time:TimeCondition")]
public class TimeCondition : Conditional
{
    [SerializeField]
    private SharedFloat waitTime=new SharedFloat(); 
    [SerializeField]
    private SharedFloat sharedTimer=new SharedFloat();  
    protected override void OnAwake() {
        sharedTimer.GetValueFromTree(tree);
        waitTime.GetValueFromTree(tree);
    }
    protected override bool IsUpdatable()
    {
        AddTimer();
        if(IsAlready())
        {
            ClearTimer();
            return true;
        }
        return false;
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
            return sharedTimer.Value>waitTime.Value;
        else
            return true;
    }
    public override void Abort()
    {
        base.Abort();
        ClearTimer();
    }
}
}
