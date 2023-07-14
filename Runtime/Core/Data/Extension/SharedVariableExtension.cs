using UnityEngine;
namespace Kurisu.AkiBT
{
    public static class SharedVariableExtension
    {
        /// <summary>
        /// Get SharedVariable from behaviorTree manually
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="tree"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SharedVariable<T> GetValueFromTree<T>(this SharedVariable<T> variable,IBehaviorTree tree)
        {
            if(variable==null)return null;
            if(!variable.IsShared)return variable;
            var value=tree.GetShareVariable<T>(variable.Name);
            if(value!=null)variable.Bind(value);
            else Debug.LogWarning($"{variable.Name} is not a valid shared variable !");  
            return variable;
        }
    }
}