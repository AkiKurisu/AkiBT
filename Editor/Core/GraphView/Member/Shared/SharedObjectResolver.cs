using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class SharedObjectResolver : FieldResolver<SharedObjectField, SharedObject>
    {
        public SharedObjectResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedObjectField CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedObjectField(fieldInfo.Name, fieldInfo.FieldType, fieldInfo);
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedObject);

    }
    public class SharedObjectField : SharedVariableField<SharedObject, UnityEngine.Object>
    {
        public SharedObjectField(string label, Type objectType, FieldInfo fieldInfo) : base(label, objectType, fieldInfo)
        {
        }
        protected override BaseField<UnityEngine.Object> CreateValueField()
        {
            Type objectType;
            try
            {
                objectType = Type.GetType(value.ConstraintTypeAQN, true);
            }
            catch
            {
                objectType = typeof(UnityEngine.Object);
            }
            return new ObjectField()
            {
                objectType = objectType
            };
        }

        protected sealed override void OnRepaint()
        {
            Type objectType;
            try
            {
                objectType = Type.GetType(value.ConstraintTypeAQN, true);
            }
            catch
            {
                objectType = typeof(UnityEngine.Object);
            }
            (ValueField as ObjectField).objectType = objectType;
        }
    }
}