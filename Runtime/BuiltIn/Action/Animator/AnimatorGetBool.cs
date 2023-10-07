using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Get the Bool element of the Animator")]
    [AkiLabel("Animator : GetBool")]
    [AkiGroup("Animator")]
    public class AnimatorGetBool : AnimatorAction
    {
        [SerializeField]
        private SharedString parameter;
        [SerializeField]
        private SharedBool storeResult;
        private int parameterHash;
        public override void Awake()
        {
            base.Awake();
            InitVariable(parameter);
            InitVariable(storeResult);
            parameterHash = Animator.StringToHash(parameter.Value);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = Animator.GetBool(parameterHash);
            return Status.Success;
        }
    }
}