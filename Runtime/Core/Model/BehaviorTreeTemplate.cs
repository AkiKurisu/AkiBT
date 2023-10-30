using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    public class BehaviorTreeTemplate
    {
        [SerializeReference]
        private List<SharedVariable> variables;
        [SerializeReference]
        private Root root;
        public List<SharedVariable> Variables => variables;
        public Root Root => root;
        public string TemplateName { get; }
#if UNITY_EDITOR
        [SerializeField]
        private List<GroupBlockData> blockData = new();
        public List<GroupBlockData> BlockData => blockData;
#endif
        public BehaviorTreeTemplate(IBehaviorTree behaviorTree)
        {
            TemplateName = behaviorTree._Object.name;
            variables = new List<SharedVariable>();
            foreach (var variable in behaviorTree.SharedVariables)
            {
                variables.Add(variable.Clone() as SharedVariable);
            }
#if UNITY_EDITOR
            blockData = behaviorTree.BlockData;
#endif
            root = behaviorTree.Root;
        }
    }
}
