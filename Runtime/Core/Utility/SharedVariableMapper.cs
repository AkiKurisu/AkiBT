using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Kurisu.AkiBT
{
    public class SharedVariableMapper
    {
        private static readonly Dictionary<Type, List<FieldInfo>> variableLookup = new();
        /// <summary>
        /// Traverse the behavior tree and automatically init all shared variables
        /// </summary>
        /// <param name="behaviorTree"></param>
        public static void Traverse(IBehaviorTree behaviorTree)
        {
            foreach (var behavior in behaviorTree.Traverse())
            {
                var behaviorType = behavior.GetType();
                if (!variableLookup.TryGetValue(behaviorType, out var fields))
                {
                    fields = behaviorType
                            .GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(x => x.FieldType.IsSubclassOf(typeof(SharedVariable)) || IsIListVariable(x.FieldType))
                            .ToList();
                    variableLookup.Add(behaviorType, fields);
                }
                foreach (var fieldInfo in fields)
                {
                    var value = fieldInfo.GetValue(behavior);
                    if (value is SharedVariable sharedVariable)
                    {
                        sharedVariable.MapTo(behaviorTree);
                    }
                    else if (value is IList sharedVariableList)
                    {
                        foreach (var variable in sharedVariableList)
                        {
                            (variable as SharedVariable).MapTo(behaviorTree);
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