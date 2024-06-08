using System;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public sealed class Ordered : Attribute
    {
        public int Order = 100;
    }
    public delegate void ValueChangeDelegate(object value);
    public interface IFieldResolver
    {
        /// <summary>
        /// Get the ValueField and bind the tree view at the same time
        /// </summary>
        /// <param name="ownerTreeView"></param>
        /// <returns></returns>
        VisualElement GetEditorField(ITreeView ownerTreeView);
        /// <summary>
        /// Only create ValueField without any binding
        /// </summary>
        /// <returns></returns>
        VisualElement CreateField();
        /// <summary>
        /// Restore editor field value from behavior
        /// </summary>
        /// <param name="behavior"></param>
        void Restore(object behavior);
        /// <summary>
        /// Commit editor field value to behavior
        /// </summary>
        /// <param name="behavior"></param>
        void Commit(object behavior);
        /// <summary>
        /// Copy field value from another resolver
        /// </summary>
        /// <param name="resolver"></param>
        void Copy(IFieldResolver resolver);
        object Value { get; set; }
        /// <summary>
        /// Register an object value change call back without knowing it's type
        /// </summary>
        /// <param name="fieldChangeCallBack"></param>
        void RegisterValueChangeCallback(ValueChangeDelegate fieldChangeCallBack);
    }
    public abstract class FieldResolver<T, K> : IFieldResolver where T : BaseField<K>
    {
        protected readonly FieldInfo fieldInfo;
        protected readonly T editorField;
        public virtual object Value
        {
            get => editorField.value;
            set => editorField.value = (K)value;
        }
        public FieldResolver(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
            editorField = CreateEditorField(fieldInfo);
            AkiLabelAttribute label = fieldInfo.GetCustomAttribute<AkiLabelAttribute>();
            if (label != null) editorField.label = label.Title;
            TooltipAttribute tooltip = fieldInfo.GetCustomAttribute<TooltipAttribute>();
            if (tooltip != null) editorField.tooltip = tooltip.tooltip;
        }
        public VisualElement CreateField()
        {
            return CreateEditorField(fieldInfo);
        }
        public VisualElement GetEditorField(ITreeView ownerTreeView)
        {
            if (editorField is IBindableField bindableField) bindableField.BindTreeView(ownerTreeView);
            return editorField;
        }
        public void Copy(IFieldResolver resolver)
        {
            if (resolver is not FieldResolver<T, K>) return;
            if (fieldInfo.GetCustomAttribute<CopyDisableAttribute>() != null) return;
            Value = resolver.Value;
        }
        public void Restore(object behavior)
        {
            Value = fieldInfo.GetValue(behavior);
        }
        public void Commit(object behavior)
        {
            fieldInfo.SetValue(behavior, Value);
        }
        public void RegisterValueChangeCallback(ValueChangeDelegate fieldChangeCallBack)
        {
            editorField.RegisterValueChangedCallback(evt => fieldChangeCallBack?.Invoke(evt.newValue));
        }
        /// <summary>
        /// Create <see cref="BaseField{T}"/>
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected abstract T CreateEditorField(FieldInfo fieldInfo);
    }
    public abstract class FieldResolver<T, K, F> : FieldResolver<T, K> where T : BaseField<K> where K : F
    {
        protected FieldResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }

        public sealed override object Value
        {
            get => ValueGetter != null ? ValueGetter(editorField.value) : editorField.value;
            set => editorField.value = ValueSetter != null ? ValueSetter((F)value) : (K)value;
        }

        /// <summary>
        /// Bridge for setting value from <see cref="K"/> to <see cref="F"/>
        /// </summary>
        /// <value></value>
        protected Func<F, K> ValueSetter { get; set; }
        /// <summary>
        /// Bridge for setting value from <see cref="K"/> to <see cref="F"/>
        /// </summary>
        /// <value></value>
        protected Func<K, object> ValueGetter { get; set; }
    }
}