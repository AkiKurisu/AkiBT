using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
    sealed class Ordered : Attribute
    {
        public int Order = 100;
    }

    public interface IFieldResolver
    {
        VisualElement GetEditorField();

        void Restore(NodeBehavior behavior);

        void Commit(NodeBehavior behavior);
    }

    public abstract class FieldResolver<T, K> :IFieldResolver where T: BaseField<K>
    {
        private readonly FieldInfo fieldInfo;
        private T editorField;

        protected FieldResolver(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            SetEditorField();
        }

        private void SetEditorField()
        {
            this.editorField = this.CreateEditorField(this.fieldInfo);
        }

        protected abstract T CreateEditorField(FieldInfo fieldInfo);

        public VisualElement GetEditorField()
        {
            return this.editorField;
        }

        public void Restore(NodeBehavior behavior)
        {
            editorField.value = (K)fieldInfo.GetValue(behavior);
        }

        public void Commit(NodeBehavior behavior)
        {
           fieldInfo.SetValue(behavior, editorField.value);
        }
    }
}