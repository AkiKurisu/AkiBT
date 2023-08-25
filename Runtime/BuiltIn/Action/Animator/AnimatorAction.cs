using UnityEngine;
namespace Kurisu.AkiBT.Extend
{

    public abstract class AnimatorAction : Action
    {
        [SerializeField, Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        private SharedTObject<Animator> animator;
        protected Animator Animator => animator.Value;
        public override void Awake()
        {
            InitVariable(animator);
            if (animator.Value == null) animator.Value = GameObject.GetComponent<Animator>();
        }
    }
}