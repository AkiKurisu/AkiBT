using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 判断类型结点行为
    /// </summary>
    public abstract class Conditional : NodeBehavior
    {
        /// <summary>
        /// 子结点运行时是否要继续判断
        /// </summary>
        [SerializeField, Tooltip("After checking, when the child node is running," +
        " the node will still be evaluated, otherwise CanUpdate will always return True when the child node is running.")]
        private bool evaluateOnRunning = false;

        [SerializeReference]
        private NodeBehavior child;

        public NodeBehavior Child
        {
            get => child;
            set => child = value;
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
        public sealed override void Dispose()
        {
            base.Dispose();
            child?.Dispose();
            child = null;
        }

        protected abstract bool IsUpdatable();

        public sealed override NodeBehavior GetChildAt(int index)
        {
            return child;
        }
        public sealed override int GetChildrenCount()
        {
            return child == null ? 0 : 1;
        }
        public sealed override void ClearChildren()
        {
            child = null;
        }
        public sealed override void AddChild(NodeBehavior nodeBehavior)
        {
            child = nodeBehavior;
        }
    }
}