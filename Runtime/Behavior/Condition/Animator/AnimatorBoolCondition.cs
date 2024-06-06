using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition: Get the Bool parameter of Animator, if it is consistent with the status, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Animator: BoolCondition")]
    [AkiGroup("Animator")]
    public class AnimatorBoolCondition : AnimatorCondition
    {
        public SharedString parameter;
        public SharedBool status;
        public SharedBool storeResult;
        private int parameterHash;
        protected override void OnAwake()
        {
            base.OnAwake();
            parameterHash = Animator.StringToHash(parameter.Value);
        }
        protected override bool IsUpdatable()
        {
            storeResult.Value = Animator.GetBool(parameterHash);
            return storeResult.Value == status.Value;
        }
    }
}