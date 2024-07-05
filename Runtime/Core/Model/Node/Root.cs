using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 根结点行为
    /// </summary>
    [AkiInfo("Root: The root of behavior tree, you can not delate it.")]
    public class Root : NodeBehavior
    {
        [SerializeReference]
        internal NodeBehavior child;
#if UNITY_EDITOR
        [HideInEditorWindow]
        public System.Action UpdateEditor;
#endif
        public NodeBehavior Child
        {
            get => child;
            set => child = value;
        }

        protected sealed override void OnRun()
        {
            child?.Run(GameObject, Tree);
        }

        public override void Awake()
        {
            child?.Awake();
        }

        public override void Start()
        {
            child?.Start();
        }

        public override void PreUpdate()
        {
            child?.PreUpdate();
        }

        protected sealed override Status OnUpdate()
        {
#if UNITY_EDITOR
            UpdateEditor?.Invoke();
#endif
            if (child == null) return Status.Failure;
            return child.Update();
        }


        public override void PostUpdate()
        {
            child?.PostUpdate();
        }

        public override void Abort()
        {
            child?.Abort();
        }
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