using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    [ResolveChild]
    public class ListResolver<T> : FieldResolver<ListField<T>, List<T>, IList<T>>
    {
        protected readonly IFieldResolver childResolver;
        public ListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo)
        {
            childResolver = resolver;
            ValueGetter = (list) =>
            {
                var iList = Activator.CreateInstance(fieldInfo.FieldType, list.Count) as IList<T>;
                bool isArray = fieldInfo.FieldType.IsArray;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (isArray)
                        iList[i] = list[i];
                    else
                        iList.Add(list[i]);
                }
                return iList;
            };
            ValueSetter = (iList) => iList != null ? iList.ToList() : new List<T>();
        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ListField<T>(fieldInfo.Name, () => childResolver.CreateField(), () => Activator.CreateInstance(typeof(T)));
        }
    }
}