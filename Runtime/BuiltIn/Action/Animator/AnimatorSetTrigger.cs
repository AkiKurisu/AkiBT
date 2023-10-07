using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Set trigger parameter of the Animator")]
    [AkiLabel("Animator : SetTrigger")]
    [AkiGroup("Animator")]
    public class AnimatorSetTrigger : AnimatorAction
    {
        [SerializeField]
        private SharedString parameter;
        private int parameterHash;
        [SerializeField]
        private bool resetLastTrigger = true;
        public override void Awake()
        {
            base.Awake();
            InitVariable(parameter);
            parameterHash = Animator.StringToHash(parameter.Value);
        }
        protected override Status OnUpdate()
        {
            if (resetLastTrigger) Animator.ResetTrigger(parameterHash);
            Animator.SetTrigger(parameterHash);
            return Status.Success;
        }
    }
}