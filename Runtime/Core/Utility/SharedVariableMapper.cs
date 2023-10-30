#if AKIBT_REFLECTION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Kurisu.AkiBT
{
    public class SharedVariableMapper
    {
        private static readonly Dictionary<Type, List<FieldInfo>> variableLookup = new();
        private static readonly Dictionary<Type, MethodInfo> initGenericMethodLookup = new();
        private static readonly MethodInfo initBaseMethod = typeof(NodeBehavior).GetMethod("InitVariableInternal", BindingFlags.Instance | BindingFlags.NonPublic);
        public static void MapSharedVariables(IBehaviorTree behaviorTree)
        {
            foreach (var behavior in behaviorTree.Traverse())
            {
                var behaviorType = behavior.GetType();
                if (!variableLookup.TryGetValue(behaviorType, out var fields))
                {
                    fields = behaviorType
                            .GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(x => x.FieldType.IsSubclassOf(typeof(SharedVariable)))
                            .ToList();
                    variableLookup.Add(behaviorType, fields);
                }
                foreach (var fieldInfo in fields)
                {
                    if (!initGenericMethodLookup.TryGetValue(fieldInfo.FieldType, out var genericMethod))
                    {
                        genericMethod = initBaseMethod.MakeGenericMethod(fieldInfo.FieldType);
                        initGenericMethodLookup.Add(fieldInfo.FieldType, genericMethod);
                    }
                    genericMethod.Invoke(behavior, new object[1] { fieldInfo.GetValue(behavior) });
                }
            }
        }
    }
}
#endif