using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:获取Animator的Bool元素")]
    [AkiLabel("Animator:GetBool")]
    [AkiGroup("Animator")]
    public class AnimatorGetBool : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private SharedBool storeResult;
        private int parameterHash;
        public override void Start() {
            parameterHash=Animator.StringToHash(parameter);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value=animator.GetBool(parameterHash);
            return Status.Success;
        }
    }
}