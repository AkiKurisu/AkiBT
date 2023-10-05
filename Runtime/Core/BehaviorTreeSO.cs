using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Behavior Tree ScriptableObject, can only run manually.
    /// </summary>
    [CreateAssetMenu(fileName = "BehaviorTreeSO", menuName = "AkiBT/BehaviorTreeSO")]
    public class BehaviorTreeSO : ScriptableObject, IBehaviorTree
    {
        [SerializeReference, HideInInspector]
        protected Root root = new();
        Object IBehaviorTree._Object => this;
        public Root Root
        {
            get => root;
#if UNITY_EDITOR
            set => root = value;
#endif
        }
        public List<SharedVariable> SharedVariables
        {
            get => sharedVariables;
#if UNITY_EDITOR
            set => sharedVariables = value;
#endif
        }
#if UNITY_EDITOR
        public virtual BehaviorTreeSO ExternalBehaviorTree => null;
        [SerializeField, HideInInspector]
        private List<GroupBlockData> blockData = new();
        public List<GroupBlockData> BlockData { get => blockData; set => blockData = value; }
        [Multiline]
        public string Description;
#endif
        [HideInInspector, SerializeReference]
        protected List<SharedVariable> sharedVariables = new();
        /// <summary>
        /// 外部传入绑定对象并初始化,调用Awake和Start方法
        /// Bind GameObject and Init behaviorTree through Awake and Start method
        /// </summary>
        /// <param name="gameObject"></param>
        public void Init(GameObject gameObject)
        {
            root.Run(gameObject, this);
            root.Awake();
            root.Start();
        }
        /// <summary>
        /// 外部调用Update更新,此方法运行完全基于SO
        /// Update BehaviorTree externally, this method is completely based on ScriptableObject
        /// </summary>
        public virtual Status Update()
        {
            root.PreUpdate();
            var status = root.Update();
            root.PostUpdate();
            return status;
        }
        public virtual void Deserialize(string serializedData)
        {
            var template = BehaviorTreeSerializeUtility.DeserializeTree(serializedData);
            if (template == null) return;
            root = template.Root;
            sharedVariables = new List<SharedVariable>(template.Variables);
#if UNITY_EDITOR
            blockData = new List<GroupBlockData>(template.BlockData);
#endif
        }
    }
}