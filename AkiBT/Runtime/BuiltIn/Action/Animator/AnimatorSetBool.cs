using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:根据status设置Animator的Bool元素")]
    [AkiLabel("Animator:SetBool")]
    [AkiGroup("Animator")]
    public class AnimatorSetBool : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private SharedBool status;
        private int parameterHash;
        public override void Start() {
            parameterHash=Animator.StringToHash(parameter);
            InitVariable(status);
        }
        protected override Status OnUpdate()
        {
            animator.SetBool(parameterHash,status.Value);
            return Status.Success;
        }
    }
}