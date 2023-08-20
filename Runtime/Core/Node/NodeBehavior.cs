using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// 行为状态
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
    public abstract class NodeBehavior
    {

#if UNITY_EDITOR
        [HideInEditorWindow]
        public Rect graphPosition = new Rect(400, 300, 100, 100);

        [HideInEditorWindow]
        public string description;

        [HideInEditorWindow]
        [NonSerialized]
        public Action<Status> NotifyEditor;
        [SerializeField, HideInEditorWindow]
        private string guid;
        public string GUID { get => guid; set => guid = value; }
#endif

        protected GameObject gameObject { private set; get; }
        protected IBehaviorTree Tree { private set; get; }
        public void Run(GameObject attachedObject, IBehaviorTree attachedTree)
        {
            gameObject = attachedObject;
            Tree = attachedTree;
            OnRun();
        }

        protected abstract void OnRun();

        public virtual void Awake() { }

        public virtual void Start() { }

        public virtual void PreUpdate() { }

        public Status Update()
        {
            var status = OnUpdate();

#if UNITY_EDITOR
            NotifyEditor?.Invoke(status);
#endif
            return status;
        }

        public virtual void PostUpdate() { }

        protected abstract Status OnUpdate();

        /// <summary>
        /// Abort running node when the condition changed.
        /// 启用Abort后Condition发生变动会调用该方法,通常用于中断逻辑
        /// </summary>
        public virtual void Abort() { }

        public virtual bool CanUpdate() => true;
        protected void InitVariable<T>(T variable) where T : SharedVariable, IBindableVariable<T>
        {
            if (variable == null) return;
            if (!variable.IsShared) return;
            //Special case of binding SharedTObject<T> to SharedObject
            if (variable is IBindableVariable<SharedObject> objectGetter)
            {
                foreach (var sharedVariable in Tree.SharedVariables)
                {
                    if (sharedVariable is SharedObject sharedObject && sharedObject.Name == variable.Name)
                    {
                        objectGetter.Bind(sharedObject);
                        return;
                    }
                }
            }
            foreach (var sharedVariable in Tree.SharedVariables)
            {
                if (sharedVariable is IBindableVariable<T> bindable && sharedVariable.Name == variable.Name)
                {
                    variable.Bind((T)bindable);
                    return;
                }
            }
            Debug.LogWarning($"{variable.Name} is not a valid shared variable !");
        }

    }
}