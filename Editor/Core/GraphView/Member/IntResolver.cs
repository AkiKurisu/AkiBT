using System;
using System.Reflection;
#if UNITY_2022_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEditor.UIElements;
#endif
namespace Kurisu.AkiBT.Editor
{
    public class IntResolver : FieldResolver<IntegerField, int>
    {
        public IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override IntegerField CreateEditorField(FieldInfo fieldInfo)
        {
            return new IntegerField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(int);
    }
}