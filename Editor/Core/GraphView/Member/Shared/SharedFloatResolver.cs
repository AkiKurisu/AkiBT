using System.Reflection;
using System;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class SharedFloatResolver : FieldResolver<SharedFloatField, SharedFloat>
    {
        public SharedFloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedFloatField CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedFloatField(fieldInfo.Name, fieldInfo.FieldType, fieldInfo);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedFloat);

    }
    public class SharedFloatField : SharedVariableField<SharedFloat, float>
    {

        public SharedFloatField(string label, Type objectType, FieldInfo fieldInfo) : base(label, objectType, fieldInfo)
        {

        }
        protected override BaseField<float> CreateValueField() => new FloatField();
    }
}