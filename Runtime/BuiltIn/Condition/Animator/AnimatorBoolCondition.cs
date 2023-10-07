using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition : Get the Bool parameter of Animator, if it is consistent with the status, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Animator : BoolCondition")]
    [AkiGroup("Animator")]
    public class AnimatorBoolCondition : AnimatorCondition
    {
        [SerializeField]
        private SharedString parameter;
        [SerializeField]
        private SharedBool status;
        [SerializeField]
        private SharedBool storeResult;
        private int parameterHash;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitVariable(parameter);
            InitVariable(status);
            InitVariable(storeResult);
            parameterHash = Animator.StringToHash(parameter.Value);
        }
        protected override bool IsUpdatable()
        {
            storeResult.Value = Animator.GetBool(parameterHash);
            return storeResult.Value == status.Value;
        }
    }
}