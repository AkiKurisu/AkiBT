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
                if (variable.Name.Equals(variableName))
                {
                    return variable;
                }
            }
            return null;
        }
        public static SharedVariable<T> GetSharedVariable<T>(this IVariableSource variableScope, string variableName)
        {
            return variableScope.GetSharedVariable(variableName) as SharedVariable<T>;
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
                if (variable.Name.Equals(variableName))
                {
                    sharedVariable = variable;
                    return true;
                }
            }
            sharedVariable = null;
            return false;
        }
        public static bool TryGetSharedVariable<T>(this IVariableSource variableScope, string variableName, out SharedVariable<T> sharedTVariable)
        {
            if (variableScope.TryGetSharedVariable(variableName, out SharedVariable sharedVariable))
            {
                sharedTVariable = sharedVariable as SharedVariable<T>;
                return sharedTVariable != null;
            }
            sharedTVariable = null;
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
                if (!variable.IsGlobal) continue;
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
            if (!variable.IsShared && !variable.IsGlobal) return;
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