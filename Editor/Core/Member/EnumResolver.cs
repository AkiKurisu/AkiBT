using System;
using System.Linq;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class EnumResolver : FieldResolver<EnumField,Enum>
    {
        public EnumResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override EnumField CreateEditorField(FieldInfo fieldInfo)
        {
            Type type=fieldInfo.FieldType;
            if(fieldInfo.FieldType.IsGenericType)type=fieldInfo.FieldType.GenericTypeArguments[0];
            var enumValue = Enum.GetValues(type).Cast<Enum>().Select(v => v).ToList();
            return new EnumField(fieldInfo.Name, enumValue, enumValue[0]);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType.IsEnum;
    }
}