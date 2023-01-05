using UnityEngine;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// 装饰器结点行为
    /// </summary>
public class Decorator : NodeBehavior
{
   

        [SerializeReference]
        private NodeBehavior child;

        public NodeBehavior Child
        {
            get => child;
#if UNITY_EDITOR
            set => child = value;
#endif
        }


        private bool isRunning = false;

        protected sealed override void OnRun()
        {
            child?.Run(gameObject,tree);
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

        public sealed override void Start()
        {
            OnStart();
            child?.Start();
        }
        
        protected virtual void OnStart()
        {
        }
        
        protected sealed override Status OnUpdate()
        {
            var status = child.Update();
            return OnDecorate(status);
        }
        /// <summary>
        /// 装饰子结点方法
        /// </summary>
        /// <param name="childStatus"></param>
        /// <returns></returns>
        protected virtual Status OnDecorate(Status childStatus)
        {
            return Status.Success;
        }
        public sealed override void PreUpdate()
        {

            child?.PreUpdate();
        }
        
        public sealed override void PostUpdate()
        {
            child?.PostUpdate();
        }
        

        public override void Abort()
        {
            if (isRunning)
            {
                isRunning = false;
                child?.Abort();
            }
        }

       
    }
}
