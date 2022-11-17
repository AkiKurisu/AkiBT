using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Float类型赋值")]
    [AkiLabel("Math:SetFloat")]
    [AkiGroup("Math")]
public class SetFloat : Action
{
    [SerializeField]
    private SharedFloat floatToSet=new SharedFloat();
    [SerializeField]
    private float setValue;
    public override void Awake() {
           floatToSet.GetValueFromTree<float>(tree);
        }
    protected override Status OnUpdate()
    {
        floatToSet.Value=setValue;
        return Status.Success;
    }
}
}