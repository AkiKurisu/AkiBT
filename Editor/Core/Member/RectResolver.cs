using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class RectResolver : FieldResolver<RectField,Rect>
    {
        public RectResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override RectField CreateEditorField(FieldInfo fieldInfo)
        {
            return new RectField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Rect);
    }
}