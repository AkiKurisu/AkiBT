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
    public class Vector2IntResolver : FieldResolver<Vector2IntField, Vector2Int>
    {
        public Vector2IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Vector2IntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new Vector2IntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(Vector2Int);
    }
}