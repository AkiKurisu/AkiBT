using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Bool类型反转")]
    [AkiLabel("Math:BoolFlip")]
    [AkiGroup("Math")]
public class BoolFlip : Action
{
    [SerializeField]
    private SharedBool boolToFlip;
    public override void Awake() {
        InitVariable(boolToFlip);
    }
    protected override Status OnUpdate()
    {
        boolToFlip.Value=!boolToFlip.Value;
        return Status.Success;
    }
}
}