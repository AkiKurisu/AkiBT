using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class StringResolver : FieldResolver<TextField,string>
    {
        private bool multiline;
        public StringResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
            multiline=fieldInfo.GetCustomAttribute<MultilineAttribute>()!=null;
        }
        protected override TextField CreateEditorField(FieldInfo fieldInfo)
        {
            var field = new TextField(fieldInfo.Name);
            field.style.minWidth = 200;
            if(multiline)
            {
                field.multiline=true;
                field.style.maxWidth=250;
                field.style.whiteSpace=WhiteSpace.Normal;
            }
            return field;
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(string);
    }
}