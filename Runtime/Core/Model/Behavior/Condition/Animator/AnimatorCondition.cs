using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    public abstract class AnimatorCondition : Conditional
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<Animator> animator;
        protected Animator Animator => animator.Value;
        protected override void OnAwake()
        {
            if (animator.Value == null) animator.Value = GameObject.GetComponent<Animator>();
        }
    }
}