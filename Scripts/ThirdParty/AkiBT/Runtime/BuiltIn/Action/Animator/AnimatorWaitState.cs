using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:等待Animator进入指定State")]
    [AkiLabel("Animator:WaitState")]
    [AkiGroup("Animator")]
    public class AnimatorWaitState : AnimatorAction
    {
        [SerializeField]
        private string stateName;
        [SerializeField]
        private int layer=-1;       
        protected override Status OnUpdate()
        {
            AnimatorStateInfo stateInfo=animator.GetCurrentAnimatorStateInfo(layer);
            if(stateInfo.IsName(stateName))
                return Status.Success;
            else
                return Status.Running;
        }
    }
}