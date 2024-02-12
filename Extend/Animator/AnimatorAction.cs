using UnityEngine;
namespace Kurisu.AkiBT.Extend.Animator
{
    public abstract class AnimatorAction : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<UnityEngine.Animator> animator;
        protected UnityEngine.Animator Animator => animator.Value;
        public sealed override void Awake()
        {
            if (animator.Value == null) animator.Value = GameObject.GetComponent<UnityEngine.Animator>();
            OnAwake();
        }
        protected virtual void OnAwake() { }
    }
}