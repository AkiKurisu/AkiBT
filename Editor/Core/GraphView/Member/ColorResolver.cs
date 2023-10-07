using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ColorResolver : FieldResolver<ColorField,Color>
    {
        public ColorResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override ColorField CreateEditorField(FieldInfo fieldInfo)
        {
            return new ColorField(fieldInfo.Name);
        }

        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(Color);
    }
}