using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Wait for the Animator to enter the specified State")]
    [AkiLabel("Animator : WaitState")]
    [AkiGroup("Animator")]
    public class AnimatorWaitState : AnimatorAction
    {
        [SerializeField]
        private SharedString stateName;
        [SerializeField]
        private SharedInt layer = new(-1);
        public override void Awake()
        {
            base.Awake();
            InitVariable(stateName);
            InitVariable(layer);
        }
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