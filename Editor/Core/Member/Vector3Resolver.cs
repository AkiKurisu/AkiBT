using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class Vector3Resolver : FieldResolver<Vector3Field,Vector3>
    {
        public Vector3Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Vector3Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new Vector3Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(Vector3);

    }
}