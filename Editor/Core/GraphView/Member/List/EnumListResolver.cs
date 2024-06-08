using System.Reflection;
using System.Collections.Generic;
using System;
namespace Kurisu.AkiBT.Editor
{
    /// <summary>
    /// Resolver for Enum <see cref="List{T}"/>, will emit specific instance by factory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumListResolver<T> : ListResolver<T> where T : Enum
    {
        public EnumListResolver(FieldInfo fieldInfo) : base(fieldInfo, new EnumResolver(fieldInfo))
        {

        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new EnumListField<T>(fieldInfo.Name, () => childResolver.CreateField(), () => default(T));
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _)
        {
            if (infoType.IsGenericType && infoType.GetGenericTypeDefinition() == typeof(List<>) && infoType.GenericTypeArguments[0].IsEnum) return true;
            if (infoType.IsArray && infoType.GetElementType().IsEnum) return true;
            return false;
        }
    }
}