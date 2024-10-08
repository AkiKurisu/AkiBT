using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Behavior Status
    /// </summary>
    public enum Status
    {
        Success,
        Failure,
        Running
    }
    /// <summary>
    /// Based Behavior Class
    /// If you want to create new types outside of Action, Conditional, Composite and Decorator,
    /// you can implement this class
    /// </summary>
    [Serializable]
    public abstract class NodeBehavior : IDisposable
    {
#if UNITY_EDITOR
        [HideInEditorWindow, NonSerialized]
        internal Action<Status> OnNotifyStatus;
        [HideInEditorWindow, NonSerialized]
        internal NodeData nodeData = new();
#endif
        protected GameObject GameObject { get; private set; }
        protected BehaviorTree Tree { get; private set; }
        public void Run(GameObject attachedObject, BehaviorTree tree)
        {
            GameObject = attachedObject;
            Tree = tree;
            OnRun();
        }

        protected abstract void OnRun();

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void PreUpdate() { }

        /// <summary>
        /// Release when behavior tree destroy
        /// </summary>
        public virtual void Dispose()
        {
            GameObject = null;
            Tree = null;
#if UNITY_EDITOR
            OnNotifyStatus = null;
#endif
        }

        public Status Update()
        {
            var status = OnUpdate();

#if UNITY_EDITOR
            OnNotifyStatus?.Invoke(status);
#endif
            return status;
        }

        public virtual void PostUpdate() { }

        protected abstract Status OnUpdate();

        /// <summary>
        /// Abort running node when the condition changed.
        /// </summary>
        public virtual void Abort() { }
        public virtual bool CanUpdate() => true;
        public virtual NodeBehavior GetChildAt(int _)
        {
            return null;
        }
        public virtual void AddChild(NodeBehavior _)
        {

        }
        public virtual int GetChildrenCount() => 0;
        public virtual void ClearChildren() { }
        public virtual NodeBehavior[] GetChildren()
        {
            return new NodeBehavior[0];
        }
        public virtual void SetChildren(NodeBehavior[] childrem) { }
#if UNITY_EDITOR
        internal NodeData GetSerializedData()
        {
            var data = nodeData.Clone();
            data.Serialize(this);
            return data;
        }
#endif
    }
}