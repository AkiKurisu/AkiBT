using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    
public abstract class AnimatorAction : Action
{
    protected Animator animator;
    public override void Awake() {
        animator=gameObject.GetComponent<Animator>();
    }
}
}