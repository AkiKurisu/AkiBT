using UnityEngine;
using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    public enum UpdateType
    {
        [InspectorName("自动模式")]
       Auto,
       [InspectorName("手动模式")]
       Manual
    }
    [DisallowMultipleComponent]
    /// <summary>
    /// Behavior Tree Component
    /// Awake, Start and Update using UnityEngine's life cycle
    /// </summary>
    public class BehaviorTree : MonoBehaviour,IBehaviorTree
    {
        
        [HideInInspector]
        [SerializeReference]
        protected Root root = new Root();
        Object IBehaviorTree._Object=>gameObject;
        [HideInInspector]
        [SerializeReference]
        protected List<SharedVariable> sharedVariables = new List<SharedVariable>();
        [SerializeField,
        Tooltip("切换成UpdateType.Manual使用手动更新并且调用BehaviorTree.Tick()")]
        private UpdateType updateType;
        #if UNITY_EDITOR
        [SerializeField,Tooltip("使用外部行为树替换组件内行为树,保存时会覆盖组件内行为树")]
        private BehaviorTreeSO externalBehaviorTree;
        public BehaviorTreeSO ExternalBehaviorTree=>externalBehaviorTree;
        [SerializeField,HideInInspector]
        private List<GroupBlockData> blockData=new List<GroupBlockData>();
        public List<GroupBlockData> BlockData { get => blockData; set=>blockData=value;}
        #endif
        public Root Root
        {
            get=>root;
            #if UNITY_EDITOR
                set => root = value;
            #endif
        }
        public List<SharedVariable> SharedVariables
        {
            get=>sharedVariables;
            #if UNITY_EDITOR
                set=>sharedVariables=value;
            #endif
        }
        SharedVariable<T> IBehaviorTree.GetShareVariable<T>(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                Debug.LogError($"共享变量名称不能为空",this);
                return null;
            }
            foreach(var variable in sharedVariables)
            {
                if(variable.Name.Equals(name))
                {
                    if( variable is SharedVariable<T>)return variable as SharedVariable<T>;
                    else Debug.LogError($"{name}名称变量不是{typeof(T).Name}类型",this);
                    return null;
                }
            }
            Debug.LogError($"没有找到共享变量:{name}",this);
            return null;
        }
        private void Awake() {
            root.Run(gameObject,this);
            root.Awake();
        }

        private void Start()
        {
            root.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }
        
        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }

    }
}