using System.Reflection;
using System.Collections.Generic;
using System;
namespace Kurisu.AkiBT.Editor
{
    public class ListResolver<T> :FieldResolver<ListField<T>,List<T>>,IChildResolver
    {
        protected readonly IFieldResolver childResolver;
        public ListResolver(FieldInfo fieldInfo,IFieldResolver resolver) : base(fieldInfo)
        {
            childResolver=resolver;
        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ListField<T>(fieldInfo.Name,null,()=>childResolver.CreateField(),
            ()=>Activator.CreateInstance(typeof(T)));
        }
    }
}