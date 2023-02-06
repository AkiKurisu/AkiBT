using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Int类型赋值")]
    [AkiLabel("Math:SetInt")]
    [AkiGroup("Math")]
public class SetInt : Action
{
    [SerializeField]
    private SharedInt intToSet;
    [SerializeField]
    private int setValue;
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