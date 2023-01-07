using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
    public class StringResolver : FieldResolver<TextField,string>
    {
        public StringResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override TextField CreateEditorField(FieldInfo fieldInfo)
        {
            var field = new TextField(fieldInfo.Name);
            field.style.minWidth = 200;
            return field;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(string);
    }
}