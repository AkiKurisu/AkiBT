using System;
using System.Linq;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public static class BehaviorNodeExtension
    {
        public static T GetSharedVariableValue<T>(this IBehaviorTreeNode node, string fieldName)
        {
            var sharedVariable = node.GetSharedVariable<SharedVariable<T>>(fieldName);
            return sharedVariable != null ? node.MapTreeView.GetSharedVariableValue(sharedVariable) : default;
        }
        public static T GetSharedVariable<T>(this IBehaviorTreeNode node, string fieldName) where T : SharedVariable
        {
            try
            {
                return (T)node.GetFieldResolver(fieldName).Value;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
        public static T GetSharedVariableValue<T>(this ITreeView treeView, SharedVariable<T> variable)
        {
            if (variable.IsShared)
            {
                if (!treeView.TryGetExposedProperty(variable.Name, out SharedVariable<T> mapContent)) return variable.Value;
                return mapContent.Value;
            }
            else
            {
                return variable.Value;
            }
        }
        public static bool TryGetExposedProperty<T>(this ITreeView treeView, string name, out T variable) where T : SharedVariable
        {
            variable = (T)treeView.SharedVariables.Where(x => x is T && x.Name.Equals(name)).FirstOrDefault();
            return variable != null;
        }
    }
}