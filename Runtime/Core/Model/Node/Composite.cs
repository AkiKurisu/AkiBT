using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 组合类型结点行为
    /// </summary>
    public abstract class Composite : NodeBehavior
    {
        [SerializeReference]
        private List<NodeBehavior> children = new();

        public List<NodeBehavior> Children => children;

        protected sealed override void OnRun()
        {
            children.ForEach(e => e.Run(GameObject, Tree));
        }

        public sealed override void Awake()
        {
            OnAwake();
            children.ForEach(e => e.Awake());
        }

        protected virtual void OnAwake()
        {
        }

        public sealed override void Start()
        {
            OnStart();
            children.ForEach(c => c.Start());
        }

        protected virtual void OnStart()
        {
        }

        public sealed override void PreUpdate()
        {
            children.ForEach(c => c.PreUpdate());
        }

        public sealed override void PostUpdate()
        {
            children.ForEach(c => c.PostUpdate());
        }

        /// <summary>
        /// 组合结点可以增加子结点
        /// </summary>
        /// <param name="child"></param>
        public sealed override void AddChild(NodeBehavior child)
        {
            children.Add(child);
        }
        public sealed override NodeBehavior GetChildAt(int index)
        {
            return children[index];
        }

        public sealed override int GetChildrenCount()
        {
            return children.Count;
        }
        public sealed override void ClearChildren()
        {
            children.Clear();
        }
    }
    /// <summary>
    /// Placeholder node for missing class node
    /// </summary>
    [AkiGroup("Hidden")]
    public sealed class InvalidNode : Composite
    {
        protected override Status OnUpdate()
        {
            return Status.Success;
        }
    }
}