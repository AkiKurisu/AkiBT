using UnityEngine;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 判断类型结点行为
    /// </summary>
    public abstract class Conditional : NodeBehavior, IIterable
    {
        /// <summary>
        /// 子结点运行时是否要继续判断
        /// </summary>
        [SerializeField, Tooltip("勾选后子结点运行时,该结点依然会进行判断,否则在子结点运行时CanUpdate总是返回True")]
        private bool evaluateOnRunning = false;

        [SerializeReference]
        private NodeBehavior child;

        public NodeBehavior Child
        {
            get => child;
#if UNITY_EDITOR
            set => child = value;
#endif
        }

        private bool? frameScope = null;

        private bool isRunning = false;

        protected sealed override void OnRun()
        {
            child?.Run(GameObject, Tree);
        }

        public sealed override void Awake()
        {
            OnAwake();
            child?.Awake();
        }
        /// <summary>
        /// Conditional Awake方法
        /// 在该方法调用后再遍历子结点
        /// </summary>
        protected virtual void OnAwake()
        {
        }

        public override void Start()
        {
            OnStart();
            child?.Start();
        }

        protected virtual void OnStart()
        {
        }

        protected override Status OnUpdate()
        {
            // no child means leaf node
            if (child == null)
            {
                return CanUpdate() ? Status.Success : Status.Failure;
            }
            if (CanUpdate())
            {
                var status = child.Update();
                isRunning = status == Status.Running;
                return status;
            }
            return Status.Failure;
        }

        public sealed override void PreUpdate()
        {
            frameScope = null;
            child?.PreUpdate();
        }

        public sealed override void PostUpdate()
        {
            frameScope = null;
            child?.PostUpdate();
        }

        public override bool CanUpdate()
        {
            if (frameScope != null)
            {
                return frameScope.Value;
            }
            //如果在运行中且没有evaluateOnRunning则直接返回正确,否则重新判断
            frameScope = isRunning && !evaluateOnRunning || IsUpdatable();
            return frameScope.Value;
        }

        public override void Abort()
        {
            if (isRunning)
            {
                isRunning = false;
                child?.Abort();
            }
        }

        protected abstract bool IsUpdatable();

        public NodeBehavior GetChildAt(int index)
        {
            return child;
        }

        public int GetChildCount()
        {
            return child == null ? 0 : 1;
        }
    }

}