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
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        private SharedObjectField editorField;
        protected override SharedObjectField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedObjectField(fieldInfo.Name, null, fieldInfo.FieldType, fieldInfo);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(SharedObject);

    }
    public class SharedObjectField : SharedVariableField<SharedObject, UnityEngine.Object>
    {
        public SharedObjectField(string label, VisualElement visualInput, Type objectType, FieldInfo fieldInfo) : base(label, visualInput, objectType, fieldInfo)
        {
        }
        protected override BaseField<UnityEngine.Object> CreateValueField()
        {
            Type objectType;
            try
            {
                objectType = Type.GetType(value.ConstraintTypeAQM, true);
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

        protected sealed override void OnValueUpdate()
        {
            Type objectType;
            try
            {
                objectType = Type.GetType(value.ConstraintTypeAQM, true);
            }
            catch
            {
                objectType = typeof(UnityEngine.Object);
            }
            (ValueField as ObjectField).objectType = objectType;
        }
    }
}