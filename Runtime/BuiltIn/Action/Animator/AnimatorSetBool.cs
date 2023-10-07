using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Set the Bool element of Animator according to status")]
    [AkiLabel("Animator : SetBool")]
    [AkiGroup("Animator")]
    public class AnimatorSetBool : AnimatorAction
    {
        [SerializeField]
        private SharedString parameter;
        [SerializeField]
        private SharedBool status;
        private int parameterHash;
        public override void Awake()
        {
            base.Awake();
            InitVariable(parameter);
            InitVariable(status);
            parameterHash = Animator.StringToHash(parameter.Value);
        }
        protected override Status OnUpdate()
        {
            Animator.SetBool(parameterHash, status.Value);
            return Status.Success;
        }
    }
}