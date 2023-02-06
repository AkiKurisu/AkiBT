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
    /// 行为树
    /// </summary>
    public class BehaviorTree : MonoBehaviour,IBehaviorTree
    {
        
        [HideInInspector]
        [SerializeReference]
        private Root root = new Root();
        Object IBehaviorTree._Object=>this;
        [HideInInspector]
        [SerializeReference]
        private List<SharedVariable> sharedVariables = new List<SharedVariable>();
        private Dictionary<string,int>sharedVariableIndex;
        [SerializeField,
        Tooltip("切换成UpdateType.Manual使用手动更新并且调用BehaviorTree.Tick()"),Header("Kurisu AkiBT行为树")]
        private UpdateType updateType;
        #if UNITY_EDITOR
        [SerializeField,Tooltip("使用外部行为树替换组件内行为树,保存时会覆盖组件内行为树")]
        private BehaviorTreeSO externalBehaviorTree;
        public BehaviorTreeSO ExternalBehaviorTree=>externalBehaviorTree;

        [SerializeField,HideInInspector]
        private bool autoSave;
        [SerializeField,HideInInspector]
        private string savePath="Assets";
        public string SavePath
        {
                get => savePath;
                set => savePath = value;

        }
        public bool AutoSave
        {
            get => autoSave;
            set => autoSave = value;
        }
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

        /// <summary>
        /// 获取共享变量
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        SharedVariable<T> IBehaviorTree.GetShareVariable<T>(string name)
        {
            if(string.IsNullOrEmpty(name))return null;
            int index;
            if(sharedVariableIndex.TryGetValue(name,out index))
            {
                var variable=sharedVariables[index];
                if(variable is SharedVariable<T>)
                {
                    return variable as SharedVariable<T>;
                }
                else
                {
                    Debug.LogError($"{name}名称变量不是{typeof(T).Name}类型");
                }
            }
            else
            {
                Debug.LogError($"没有在行为树中找到共享变量:{name}");
            }
            return null;
        }
        private void Awake() {
            sharedVariableIndex=new Dictionary<string, int>();
            for(int i=0;i<sharedVariables.Count;i++)
            {
                sharedVariableIndex.Add(sharedVariables[i].Name,i);
            }
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