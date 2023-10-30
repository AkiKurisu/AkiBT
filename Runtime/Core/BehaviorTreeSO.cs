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
            var template = SerializeUtility.DeserializeTree(serializedData);
            if (template == null) return;
            root = template.Root;
            sharedVariables = new List<SharedVariable>(template.Variables);
#if UNITY_EDITOR
            blockData = new List<GroupBlockData>(template.BlockData);
#endif
        }
#if AKIBT_REFLECTION
        /// <summary>
        /// This will be called when the object is loaded for the first time when entering PlayMode
        /// Currently we only need to map variables once per scriptableObject
        /// </summary>
        //TODO: After adding Scene-Scope BlackBoard and Game-Scope BlackBoard, map time will be changed
        private void Awake()
        {
            root.Run(null, this);
            SharedVariableMapper.MapSharedVariables(this);
        }
#endif
    }
}