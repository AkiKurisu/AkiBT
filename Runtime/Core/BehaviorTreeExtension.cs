using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    public static class BehaviorTreeExtension
    {
        /// <summary>
        /// Get shared variable by its name
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <returns></returns>
        public static SharedVariable GetSharedVariable(this IBehaviorTree behaviorTree, string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                Debug.LogError($"Shared variable name cannot be empty", behaviorTree._Object);
                return null;
            }
            foreach (var variable in behaviorTree.SharedVariables)
            {
                if (variable.Name.Equals(variableName))
                {
                    return variable;
                }
            }
            Debug.LogError($"Can't find shared variable : {variableName}", behaviorTree._Object);
            return null;
        }
        public static TraverseIterator Traverse(this IBehaviorTree behaviorTree)
        {
            return new TraverseIterator(behaviorTree);
        }
    }
}