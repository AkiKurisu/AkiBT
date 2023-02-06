using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kurisu.AkiBT.Editor
{
    public interface IChildResolver
    {

    }
    /// <summary>
    /// 字段解析工厂
    /// </summary>
    public class FieldResolverFactory
    {
        private List<Type> _ResolverTypes = new List<Type>();
        private List<Type> _ResolverGenericTypes=new List<Type>();
        public FieldResolverFactory()
        {
            _ResolverTypes=AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x=>x.GetTypes())
            .SelectMany(x=>x)
            .Where(x=>IsValidType(x))
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
            if (type.GetMethod("IsAcceptable") == null)return false;
            if (type == typeof(ObjectResolver)) return false;
            if (!type.GetInterfaces().Any(t => t == typeof(IFieldResolver))) return false;
            return true;
        }
        public IFieldResolver Create(FieldInfo fieldInfo)
        {
            Type fieldType=fieldInfo.FieldType;
            Type parameterType=fieldType.IsGenericType?fieldType.GenericTypeArguments[0]:fieldType;
            foreach (var _type in _ResolverTypes) {
                var resolverType=_type;
                //判断是否为未构造泛型T
                if(resolverType.IsGenericTypeDefinition)
                {
                    try
                    {
                        resolverType=resolverType.MakeGenericType(parameterType);//尝试构造为resolverType<parameterType>
                    }
                    catch
                    { 
                        continue;//没有通过约束则忽略
                    }
                }
                if(!IsAcceptable(resolverType,fieldType,fieldInfo))continue;//不满足则继续
                if(resolverType.GetInterfaces().Any(t => t == typeof(IChildResolver)))//判断是否需要子解析器
                return (IFieldResolver)Activator.CreateInstance(resolverType, new object[]{fieldInfo,GetChildResolver(parameterType,fieldInfo)});//2个元素的构造函数
                return (IFieldResolver)Activator.CreateInstance(resolverType, new object[]{fieldInfo});//非列表解析器为1个元素的构造函数
            }
            if(!IsList(fieldType))return new ObjectResolver(fieldInfo);//非列表类型字段返回对象解析器

            IFieldResolver childResolver=GetChildResolver(parameterType,fieldInfo);//列表类型字段获取子解析器
            if(childResolver==null)return (IFieldResolver)Activator.CreateInstance(typeof(ObjectListResolver<>).MakeGenericType(parameterType), new object[]{fieldInfo});
            //无子解析器返回对象列表解析器
            //有则返回包含子解析器的列表解析器
            return (IFieldResolver)Activator.CreateInstance(typeof(ListResolver<>).MakeGenericType(parameterType), new object[]{fieldInfo,childResolver});
        }
        /// <summary>
        /// 类型是列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsList(Type type)
        {
            return type.IsGenericType&&type.GetGenericTypeDefinition()==typeof(List<>);
        }
        /// <summary>
        /// 判断可以解析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterType">类型</param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static bool IsAcceptable(Type type,Type fieldType,FieldInfo fieldInfo)
        {
            if(type.IsGenericTypeDefinition)return false;//跳过未构造的泛型T类型
            return (bool)type.InvokeMember("IsAcceptable", BindingFlags.InvokeMethod, null, null,new object[] {fieldType,fieldInfo});
        }
        /// <summary>
        /// 获取子字段解析
        /// </summary>
        /// <param name="childFieldType"></param>
        /// <param name="fatherFieldInfo"></param>
        /// <returns></returns>
        private IFieldResolver GetChildResolver(Type childFieldType,FieldInfo fatherFieldInfo)
        {
            foreach (var resolverType in _ResolverTypes) {
                if(IsAcceptable(resolverType,childFieldType,fatherFieldInfo))
                return (IFieldResolver)Activator.CreateInstance(resolverType, new object[]{fatherFieldInfo});
            }
            return null;
        }
    }
}