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
    public class Vector2Resolver : FieldResolver<Vector2Field, Vector2>
    {
        public Vector2Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Vector2Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new Vector2Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(Vector2);
    }
}