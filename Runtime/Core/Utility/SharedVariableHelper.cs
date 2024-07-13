using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Kurisu.AkiBT
{
    public static class SharedVariableHelper
    {
        private static readonly Dictionary<Type, List<FieldInfo>> fieldInfoLookup = new();
        /// <summary>
        /// Traverse the behavior tree and automatically init all shared variables
        /// </summary>
        /// <param name="behaviorTree>
        public static void InitVariables(BehaviorTree behaviorTree)
        {
            HashSet<SharedVariable> internalVariables = behaviorTree.internalVariables;
            foreach (var behavior in behaviorTree)
            {
                var behaviorType = behavior.GetType();
                if (!fieldInfoLookup.TryGetValue(behaviorType, out var fields))
                {
                    fields = behaviorType
                            .GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(x => x.FieldType.IsSubclassOf(typeof(SharedVariable)) || IsIListVariable(x.FieldType))
                            .ToList();
                    fieldInfoLookup.Add(behaviorType, fields);
                }
                foreach (var fieldInfo in fields)
                {
                    var value = fieldInfo.GetValue(behavior);
                    // shared variables should not be null (dsl/builder will cause null variables)
                    if (value == null)
                    {
                        value = Activator.CreateInstance(fieldInfo.FieldType);
                        fieldInfo.SetValue(behavior, value);
                    }
                    if (value is SharedVariable sharedVariable)
                    {
                        sharedVariable.MapTo(behaviorTree.BlackBoard);
                        internalVariables.Add(sharedVariable);
                    }
                    else if (value is IList sharedVariableList)
                    {
                        foreach (var variable in sharedVariableList)
                        {
                            var sv = variable as SharedVariable;
                            internalVariables.Add(sv);
                            sv.MapTo(behaviorTree.BlackBoard);
                        }
                    }
                }
            }
        }
        private static bool IsIListVariable(Type fieldType)
        {
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type genericArgument = fieldType.GetGenericArguments()[0];
                if (typeof(SharedVariable).IsAssignableFrom(genericArgument))
                {
                    return true;
                }
            }
            else if (fieldType.IsArray)
            {
                Type elementType = fieldType.GetElementType();
                if (typeof(SharedVariable).IsAssignableFrom(elementType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}