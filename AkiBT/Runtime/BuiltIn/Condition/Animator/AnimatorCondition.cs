using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    
public abstract class AnimatorCondition : Conditional
{
    protected Animator animator;
    protected override void OnAwake() {
        animator=gameObject.GetComponent<Animator>();
    }
}
}