using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Wait for the Animator to enter the specified State")]
    [AkiLabel("Animator: WaitState")]
    [AkiGroup("Animator")]
    public class AnimatorWaitState : AnimatorAction
    {
        public SharedString stateName;
        public SharedInt layer = new(-1);
        protected override Status OnUpdate()
        {
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(layer.Value);
            if (stateInfo.IsName(stateName.Value))
                return Status.Success;
            else
                return Status.Running;
        }
    }
}