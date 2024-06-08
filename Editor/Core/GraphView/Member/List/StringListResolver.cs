using System;
using System.Collections.Generic;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class StringListResolver : ListResolver<string>
    {
        public StringListResolver(FieldInfo fieldInfo) : base(fieldInfo, new StringResolver(fieldInfo))
        {

        }
        protected override ListField<string> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ListField<string>(fieldInfo.Name, () => childResolver.CreateField(), () => string.Empty);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(List<string>) || infoType == typeof(string[]);
    }
}