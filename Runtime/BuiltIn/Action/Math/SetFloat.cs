using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Float类型赋值")]
    [AkiLabel("Math:SetFloat")]
    [AkiGroup("Math")]
public class SetFloat : Action
{
    [SerializeField]
    private float setValue;
    [SerializeField,ForceShared]
    private SharedFloat floatToSet;
    public override void Awake() {
        InitVariable(floatToSet);
    }
    protected override Status OnUpdate()
    {
        floatToSet.Value=setValue;
        return Status.Success;
    }
}
}