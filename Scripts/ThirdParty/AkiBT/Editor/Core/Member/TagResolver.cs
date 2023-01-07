using System;
using System.Reflection;
using UnityEditor.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [Ordered]
    public class TagResolver : FieldResolver<TagField,string>
    {
        public TagResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override TagField CreateEditorField(FieldInfo fieldInfo)
        {
            return new TagField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(string) && info.GetCustomAttribute<Tag>() != null;
    }
}