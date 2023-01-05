using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:触发Animator的Trigger元素")]
    [AkiLabel("Animator:SetTrigger")]
    [AkiGroup("Animator")]
    public class AnimatorSetTrigger : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        private int parameterHash;
        [SerializeField]
        private bool resetLastTrigger=true;
        public override void Start() {
            parameterHash=Animator.StringToHash(parameter);
        }
        protected override Status OnUpdate()
        {
            if(resetLastTrigger)animator.ResetTrigger(parameterHash);
            animator.SetTrigger(parameterHash);
            return Status.Success;
        }
    }
}