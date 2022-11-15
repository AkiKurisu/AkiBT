using System.Reflection;
using UnityEditor.UIElements;

namespace Kurisu.AkiBT.Editor
{
    [Ordered]
    public class LayerResolver : FieldResolver<LayerField,int>
    {
        public LayerResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override LayerField CreateEditorField(FieldInfo fieldInfo)
        {
            return new LayerField(fieldInfo.Name);
        }
        public static bool IsAcceptable(FieldInfo info) => info.FieldType == typeof(int) && info.GetCustomAttribute<Layer>() != null;
    }
}