using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
[AkiInfo("Action:Int类型转为Float类型")]
[AkiLabel("Math:Int2Float")]
[AkiGroup("Math")]
public class Int2Float : Action
{
    [SerializeField]
    private SharedInt value;
    [SerializeField]
    private SharedFloat newValue;
    public override void Awake() {
        InitVariable(value);
        InitVariable(newValue);
    }
     protected override Status OnUpdate()
    {
        newValue.Value=(float)value.Value;
        return Status.Success;
    }
}
}
