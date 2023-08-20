using System;
using System.Reflection;
#if UNITY_2022_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEditor.UIElements;
#endif
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class RectResolver : FieldResolver<RectField, Rect>
    {
        public RectResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override RectField CreateEditorField(FieldInfo fieldInfo)
        {
            return new RectField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(Rect);
    }
}