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
                protected internal Root root = new();
                public Root Root => root;
                Object IBehaviorTree._Object => this;
                public List<SharedVariable> SharedVariables => sharedVariables;
#if UNITY_EDITOR
                [SerializeField, HideInInspector]
                private List<GroupBlockData> blockData = new();
                public List<GroupBlockData> BlockData => blockData;
                [Multiline]
                public string Description;
#endif
                [HideInInspector, SerializeReference]
                protected List<SharedVariable> sharedVariables = new();
                /// <summary>
                /// Whether behaviorTreeSO is initialized
                /// </summary>
                /// <value></value>
                public bool IsInitialized { get; private set; }
                /// <summary>
                /// Bind GameObject and Init behaviorTree through Awake and Start method
                /// </summary>
                /// <param name="gameObject"></param>
                public void Init(GameObject gameObject)
                {
                        if (!IsInitialized)
                        {
                                Initialize();
                        }
                        this.MapGlobal();
                        root.Run(gameObject, this);
                        root.Awake();
                        root.Start();
                }
                /// <summary>
                /// Update BehaviorTree externally
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
                        IsInitialized = true;
                        SharedVariableMapper.Traverse(this);
                }
                /// <summary>
                /// This will be called when the object is loaded for the first time when entering PlayMode
                /// Currently we only need to map variables once per scriptableObject
                /// </summary>
                private void Awake()
                {
                        if (!IsInitialized)
                        {
                                Initialize();
                        }
                }
                /// <summary>
                /// Traverse behaviors and bind shared variables
                /// </summary>
                public void Initialize()
                {
                        IsInitialized = true;
                        SharedVariableMapper.Traverse(this);
                }
        }
}