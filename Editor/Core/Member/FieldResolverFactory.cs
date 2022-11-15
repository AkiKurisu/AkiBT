using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kurisu.AkiBT.Editor
{
    /// <summary>
    /// 字段解析工厂
    /// </summary>
    public class FieldResolverFactory
    {
        private List<Type> _ResolverTypes = new List<Type>();

        public FieldResolverFactory()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (type.GetMethod("IsAcceptable") == null) continue;
                    if (type == typeof(ObjectResolver)) continue;
                    if (!type.GetInterfaces().Any(t => t == typeof(IFieldResolver))) continue;
                    _ResolverTypes.Add(type);
                }
            }
            _ResolverTypes.Sort((a, b) =>
            {
                var aOrdered = a.GetCustomAttribute<Ordered>();
                var bOrdered = b.GetCustomAttribute<Ordered>();
                if (aOrdered == null && bOrdered == null) return 0;
                if (aOrdered != null && bOrdered != null) return aOrdered.Order - bOrdered.Order;
                if (aOrdered != null) return -1;
                return 1;
            });
        }

        public IFieldResolver Create(FieldInfo fieldInfo)
        {
            foreach (var resolverType in _ResolverTypes) {
                bool isAcceptable = (bool)resolverType.InvokeMember("IsAcceptable", BindingFlags.InvokeMethod, null, null,
                    new object[] {fieldInfo});
                if (isAcceptable)
                {
                    return (IFieldResolver)Activator.CreateInstance(resolverType, new object[]{fieldInfo});
                }
            }
            return new ObjectResolver(fieldInfo);
        }
    }
}