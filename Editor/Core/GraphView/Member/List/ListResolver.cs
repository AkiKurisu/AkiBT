using System.Reflection;
using System.Collections.Generic;
using System;
namespace Kurisu.AkiBT.Editor
{
    [ResolveChild]
    public class ListResolver<T> : FieldResolver<ListField<T>, List<T>>
    {
        protected readonly IFieldResolver childResolver;
        public ListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo)
        {
            childResolver = resolver;
        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ListField<T>(fieldInfo.Name, null, () => childResolver.CreateField(),
            () => Activator.CreateInstance(typeof(T)));
        }
    }
}