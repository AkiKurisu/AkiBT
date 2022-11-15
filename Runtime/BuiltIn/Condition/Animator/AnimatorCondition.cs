using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    
public abstract class AnimatorCondition : Conditional
{
    protected Animator animator;
    protected override void OnStart() {
        animator=gameObject.GetComponent<Animator>();
    }
}
}