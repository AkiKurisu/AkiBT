using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Int类型赋值")]
    [AkiLabel("Math:SetInt")]
    [AkiGroup("Math")]
public class SetInt : Action
{
    [SerializeField]
    private int setValue;
    [SerializeField,ForceShared]
    private SharedInt intToSet;
    
    public override void Awake() {
        InitVariable(intToSet);
    }
    protected override Status OnUpdate()
    {
        intToSet.Value=setValue;
        return Status.Success;
    }
}
}