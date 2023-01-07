using System;
using System.Reflection;
using UnityEditor.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class FloatResolver : FieldResolver<FloatField,float>
    {
        public FloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override FloatField CreateEditorField(FieldInfo fieldInfo)
        {
            return new FloatField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(float);
    }
}