using System;
using System.Reflection;
#if UNITY_2022_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEditor.UIElements;
#endif
namespace Kurisu.AkiBT.Editor
{
    public class FloatResolver : FieldResolver<FloatField, float>
    {
        public FloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override FloatField CreateEditorField(FieldInfo fieldInfo)
        {
            return new FloatField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(float);
    }
}