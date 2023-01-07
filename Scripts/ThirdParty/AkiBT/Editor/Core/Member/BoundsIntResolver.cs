using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BoundsIntResolver : FieldResolver<BoundsIntField,BoundsInt>
    {
        public BoundsIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override BoundsIntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new BoundsIntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(BoundsInt);
    }
}