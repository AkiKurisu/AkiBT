using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class Vector3IntResolver : FieldResolver<Vector3IntField,Vector3Int>
    {
        public Vector3IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Vector3IntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new Vector3IntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(Vector3Int);

    }
}