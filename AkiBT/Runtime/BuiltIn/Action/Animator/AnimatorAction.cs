using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    
public abstract class AnimatorAction : Action
{
    [SerializeField,Tooltip("如不填写,则从绑定物体中获取")]
    private Animator animator;
    protected Animator _Animator=>animator;
    public override void Awake() {
        if(animator==null)animator=gameObject.GetComponent<Animator>();
    }
}
}