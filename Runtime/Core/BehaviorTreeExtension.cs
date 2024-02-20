using UnityEngine;
namespace Kurisu.AkiBT
{
    public static class BehaviorTreeExtension
    {
        /// <summary>
        /// Get shared variable by it's name
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <returns></returns>
        public static SharedVariable GetSharedVariable(this IVariableSource variableScope, string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                return null;
            }
            foreach (var variable in variableScope.SharedVariables)
            {
                if (variableName == variable.Name)
                {
                    return variable;
                }
            }
            return null;
        }
        public static SharedVariable<T> GetSharedVariable<T>(this IVariableSource variableScope, string variableName) where T : unmanaged
        {
            return variableScope.GetSharedVariable(variableName) as SharedVariable<T>;
        }
        public static SharedVariable<string> GetSharedString(this IVariableSource variableScope, string variableName)
        {
            return variableScope.GetSharedVariable(variableName) as SharedVariable<string>;
        }
        public static SharedVariable<T> GetSharedObject<T>(this IVariableSource variableScope, string variableName) where T : Object
        {
            return variableScope.GetSharedVariable(variableName) as SharedVariable<T>;
        }
        public static SharedVariable<Object> GetSharedObject(this IVariableSource variableScope, string variableName)
        {
            return variableScope.GetSharedVariable(variableName) as SharedVariable<Object>;
        }
        public static T GetValue<T>(this IVariableSource variableScope, string variableName) where T : unmanaged
        {
            var variable = variableScope.GetSharedVariable(variableName);
            if (variable is SharedVariable<T> tVariable) return tVariable.Value;
            return default;
        }
        public static T GetObject<T>(this IVariableSource variableScope, string variableName) where T : Object
        {
            var variable = variableScope.GetSharedVariable(variableName);
            if (variable is SharedVariable<T> tVariable) return tVariable.Value;
            if (variable is SharedVariable<Object> sharedObject) return sharedObject.Value as T;
            return null;
        }
        public static string GetString(this IVariableSource variableScope, string variableName)
        {
            var variable = variableScope.GetSharedVariable(variableName);
            if (variable is SharedVariable<string> sVariable) return sVariable.Value;
            return null;
        }
        /// <summary>
        /// Try get shared variable by it's name
        /// </summary>
        /// <param name="variableScope"></param>
        /// <param name="variableName"></param>
        /// <param name="sharedVariable"></param>
        /// <returns></returns>
        public static bool TryGetSharedVariable(this IVariableSource variableScope, string variableName, out SharedVariable sharedVariable)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                sharedVariable = null;
                return false;
            }
            foreach (var variable in variableScope.SharedVariables)
            {
                if (variableName == variable.Name)
                {
                    sharedVariable = variable;
                    return true;
                }
            }
            sharedVariable = null;
            return false;
        }
        public static bool TryGetSharedVariable<T>(this IVariableSource variableScope, string variableName, out SharedVariable<T> sharedTVariable) where T : unmanaged
        {
            if (variableScope.TryGetSharedVariable(variableName, out SharedVariable sharedVariable))
            {
                sharedTVariable = sharedVariable as SharedVariable<T>;
                return sharedTVariable != null;
            }
            sharedTVariable = null;
            return false;
        }
        public static bool TryGetSharedString(this IVariableSource variableScope, string variableName, out SharedVariable<string> sharedTVariable)
        {
            if (variableScope.TryGetSharedVariable(variableName, out SharedVariable sharedVariable))
            {
                sharedTVariable = sharedVariable as SharedVariable<string>;
                return sharedTVariable != null;
            }
            sharedTVariable = null;
            return false;
        }
        public static bool TryGetSharedObject(this IVariableSource variableScope, string variableName, out SharedVariable<Object> sharedObject)
        {
            if (variableScope.TryGetSharedVariable(variableName, out SharedVariable sharedVariable))
            {
                sharedObject = sharedVariable as SharedVariable<Object>;
                return sharedObject != null;
            }
            sharedObject = null;
            return false;
        }
        public static bool TryGetSharedObject<T>(this IVariableSource variableScope, string variableName, out SharedVariable<T> sharedTObject) where T : Object
        {
            if (variableScope.TryGetSharedVariable(variableName, out SharedVariable sharedVariable))
            {
                sharedTObject = sharedVariable as SharedVariable<T>;
                return sharedTObject != null;
            }
            sharedTObject = null;
            return false;
        }
        /// <summary>
        /// Map variable to global variables
        /// </summary>
        /// <param name="variableSource"></param>
        public static void MapGlobal(this IVariableSource variableSource)
        {
            var globalVariables = GlobalVariables.Instance;
            foreach (var variable in variableSource.SharedVariables)
            {
                if (!variable.IsGlobal || !variable.IsExposed) continue;
                variable.MapTo(globalVariables);
            }
        }
        /// <summary>
        /// Map variable to target variable source if variable is shared or is global
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="variableSource"></param>
        public static void MapTo(this SharedVariable variable, IVariableSource variableSource)
        {
            if (variable == null) return;
            if (!variable.IsShared && !variable.IsGlobal && !variable.IsExposed) return;
            if (!variableSource.TryGetSharedVariable(variable.Name, out SharedVariable sharedVariable)) return;
            variable.Bind(sharedVariable);
        }
        public static TraverseIterator Traverse(this IBehaviorTree behaviorTree, bool includeChildren = true)
        {
            return new TraverseIterator(behaviorTree.Root, includeChildren);
        }
        public static TraverseIterator Traverse(this NodeBehavior nodeBehavior, bool includeChildren = true)
        {
            return new TraverseIterator(nodeBehavior, includeChildren);
        }
    }
}