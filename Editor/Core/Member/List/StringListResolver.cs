using System;
using System.Collections.Generic;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class StringListResolver : ListResolver<string>
    {
        public StringListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo, resolver)
        {

        }
        protected override ListField<string> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ListField<string>(fieldInfo.Name, null, () => childResolver.CreateField(),
            () => string.Empty);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(List<string>);
    }
}