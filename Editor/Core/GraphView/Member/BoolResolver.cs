using System;
using System.Reflection;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class BoolResolver : FieldResolver<Toggle,bool>
    {
        public BoolResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override Toggle CreateEditorField(FieldInfo fieldInfo)
        {
            return new Toggle(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(bool);
    }
}