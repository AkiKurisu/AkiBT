using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 根结点行为
    /// </summary>
    [AkiInfo("Root:根节点,你不能删除它")]
    public class Root : NodeBehavior, IIterable
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
#if UNITY_EDITOR
            set => child = value;
#endif
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