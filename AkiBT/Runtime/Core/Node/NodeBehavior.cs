using System;
using UnityEngine;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 行为状态
    /// </summary>
    public enum Status
    {
        Success,
        Failure,
        Running
    }
   /// <summary>
   /// 基础的结点行为类
   /// </summary>
    [Serializable]
    public abstract class NodeBehavior 
    {

        #if UNITY_EDITOR
        [HideInEditorWindow]
        public Rect graphPosition = new Rect(400,300,100,100);
        
        [HideInEditorWindow]
        public string description;
        
        [HideInEditorWindow]
        [NonSerialized]
        public Action<Status> NotifyEditor;
        [SerializeField,HideInEditorWindow]
        private string guid;
        public string GUID{get=>guid;set=>guid=value;}
        #endif

        protected GameObject gameObject { private set; get; }
        protected IBehaviorTree tree{ private set; get; }
        public void Run(GameObject attachedObject,IBehaviorTree attachedTree)
        {
            gameObject = attachedObject;
            tree=attachedTree;
            OnRun();
        }
        
        protected abstract void OnRun();

        public virtual void Awake() { }

        public virtual void Start() {}

        public virtual void PreUpdate() {}

        public Status Update()
        {
            var status = OnUpdate();

#if UNITY_EDITOR
            NotifyEditor?.Invoke(status);
#endif
            return status;
        }

        public virtual void PostUpdate(){}

        protected abstract Status OnUpdate();

        /// <summary>
        ///  abort running node when the condition changed.
        /// 启用Abort后Condition发生变动会调用该方法,通常用于中断逻辑
        /// </summary>
        public virtual void Abort() {}
        
        public virtual bool CanUpdate() => true;
        /// <summary>
        /// 初始化共享变量
        /// </summary>
        /// <param name="shareVariable"></param>
        /// <typeparam name="T"></typeparam>
        protected  void InitVariable<T>(SharedVariable<T> shareVariable)
        {
            shareVariable.GetValueFromTree(tree);
        }
        
    }
}