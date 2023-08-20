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
    public class Vector4Resolver : FieldResolver<Vector4Field, Vector4>
    {
        public Vector4Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Vector4Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new Vector4Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(Vector4);

    }
}