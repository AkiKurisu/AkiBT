using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    public abstract class AnimatorCondition : Conditional
    {
        [SerializeField, Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        private SharedTObject<Animator> animator;
        protected Animator Animator => animator.Value;
        protected override void OnAwake()
        {
            InitVariable(animator);
            if (animator.Value == null) animator.Value = GameObject.GetComponent<Animator>();
        }
    }
}