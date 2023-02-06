using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Bool类型赋值")]
    [AkiLabel("Math:SetBool")]
    [AkiGroup("Math")]
public class SetBool : Action
{
    [SerializeField]
    private SharedBool boolToSet;
    [SerializeField]
    private bool setValue;
    public override void Awake() {
        InitVariable(boolToSet);
    }
    protected override Status OnUpdate()
    {
        boolToSet.Value=setValue;
        return Status.Success;
    }
}
}