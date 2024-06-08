using System.Reflection;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif
using System;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class SharedVector3Resolver : FieldResolver<SharedVector3Field, SharedVector3>
    {
        public SharedVector3Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedVector3Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedVector3Field(fieldInfo.Name, fieldInfo.FieldType, fieldInfo);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedVector3);
    }
    public class SharedVector3Field : SharedVariableField<SharedVector3, Vector3>
    {
        public SharedVector3Field(string label, Type objectType, FieldInfo fieldInfo) : base(label, objectType, fieldInfo)
        {
        }
        protected override BaseField<Vector3> CreateValueField() => new Vector3Field();
    }
}
