using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class LayerMaskResolver : FieldResolver<LayerMaskField,int>
    {
        public LayerMaskResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override LayerMaskField CreateEditorField(FieldInfo fieldInfo)
        {
            return new LayerMaskField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(LayerMask);
    }
}