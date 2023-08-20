using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ResolveChildAttribute : Attribute
    {

    }
    public class FieldResolverFactory
    {
        private static FieldResolverFactory instance;
        public static FieldResolverFactory Instance => instance ?? new FieldResolverFactory();

        private readonly List<Type> _ResolverTypes = new();
        public FieldResolverFactory()
        {
            instance = this;
            _ResolverTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.GetTypes())
            .SelectMany(x => x)
            .Where(x => IsValidType(x))
            .ToList();
            _ResolverTypes.Sort((a, b) =>
            {
                var aOrdered = a.GetCustomAttribute<Ordered>(false);
                var bOrdered = b.GetCustomAttribute<Ordered>(false);
                if (aOrdered == null && bOrdered == null) return 0;
                if (aOrdered != null && bOrdered != null) return aOrdered.Order - bOrdered.Order;
                if (aOrdered != null) return -1;
                return 1;
            });
        }
        static bool IsValidType(Type type)
        {
            if (type.IsAbstract) return false;
            if (type.GetMethod("IsAcceptable") == null) return false;
            if (type == typeof(ObjectResolver)) return false;
            if (!type.GetInterfaces().Any(t => t == typeof(IFieldResolver))) return false;
            return true;
        }
        public IFieldResolver Create(FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;
            Type parameterType = fieldType.IsGenericType ? fieldType.GenericTypeArguments[0] : fieldType;
            foreach (var _type in _ResolverTypes)
            {
                var resolverType = _type;
                //Try create generic resolver for this field type, can be more easier for user to create custom field
                if (resolverType.IsGenericTypeDefinition)
                {
                    try
                    {
                        resolverType = resolverType.MakeGenericType(parameterType);
                    }
                    catch
                    {
                        continue;
                    }
                }
                if (!IsAcceptable(resolverType, fieldType, fieldInfo)) continue;
                //Identify the list field whether should resolve it's child
                if (resolverType.GetCustomAttribute(typeof(ResolveChildAttribute)) != null)
                    return (IFieldResolver)Activator.CreateInstance(resolverType, new object[] { fieldInfo, GetChildResolver(parameterType, fieldInfo) });
                else
                    return (IFieldResolver)Activator.CreateInstance(resolverType, new object[] { fieldInfo });
            }
            if (!IsList(fieldType))
                return new ObjectResolver(fieldInfo);
            //Special case : List<Object>
            IFieldResolver childResolver = GetChildResolver(parameterType, fieldInfo);
            if (childResolver == null)
                return (IFieldResolver)Activator.CreateInstance(typeof(ObjectListResolver<>).MakeGenericType(parameterType), new object[] { fieldInfo });
            else
                return (IFieldResolver)Activator.CreateInstance(typeof(ListResolver<>).MakeGenericType(parameterType), new object[] { fieldInfo, childResolver });
        }
        public static bool IsSharedTObject(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SharedTObject<>);
        }
        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
        private static bool IsAcceptable(Type type, Type fieldType, FieldInfo fieldInfo)
        {
            //Skip unconstructed generic T type
            if (type.IsGenericTypeDefinition) return false;
            return (bool)type.InvokeMember("IsAcceptable", BindingFlags.InvokeMethod, null, null, new object[] { fieldType, fieldInfo });
        }
        private IFieldResolver GetChildResolver(Type childFieldType, FieldInfo fatherFieldInfo)
        {
            foreach (var resolverType in _ResolverTypes)
            {
                if (IsAcceptable(resolverType, childFieldType, fatherFieldInfo))
                    return (IFieldResolver)Activator.CreateInstance(resolverType, new object[] { fatherFieldInfo });
            }
            return null;
        }
    }
}