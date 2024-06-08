using System.Reflection;
using System;
using UnityEngine.UIElements;
#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif
namespace Kurisu.AkiBT.Editor
{

    public class SharedBoolResolver : FieldResolver<SharedBoolField, SharedBool>
    {
        public SharedBoolResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedBoolField CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedBoolField(fieldInfo.Name, fieldInfo.FieldType, fieldInfo);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedBool);

    }
    public class SharedBoolField : SharedVariableField<SharedBool, bool>
    {
        public SharedBoolField(string label, Type objectType, FieldInfo fieldInfo) : base(label, objectType, fieldInfo)
        {


        }

        protected override BaseField<bool> CreateValueField() => new Toggle("Value");
    }
}
