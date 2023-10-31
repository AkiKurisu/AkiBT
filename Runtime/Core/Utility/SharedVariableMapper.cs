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
                            .Where(x => x.FieldType.IsSubclassOf(typeof(SharedVariable)))
                            .ToList();
                    variableLookup.Add(behaviorType, fields);
                }
                foreach (var fieldInfo in fields)
                {
                    (fieldInfo.GetValue(behavior) as SharedVariable).MapToInternal(behaviorTree);
                }
            }
        }
    }
}
#endif