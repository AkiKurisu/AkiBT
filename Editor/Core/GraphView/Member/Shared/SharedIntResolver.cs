using System.Reflection;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class SharedIntResolver : FieldResolver<SharedIntField, SharedInt>
    {
        public SharedIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedIntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedIntField(fieldInfo.Name, fieldInfo.FieldType, fieldInfo);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedInt);

    }
    public class SharedIntField : SharedVariableField<SharedInt, int>
    {

        public SharedIntField(string label, Type objectType, FieldInfo fieldInfo) : base(label, objectType, fieldInfo)
        {
        }
        protected override BaseField<int> CreateValueField() => new IntegerField();
    }
}