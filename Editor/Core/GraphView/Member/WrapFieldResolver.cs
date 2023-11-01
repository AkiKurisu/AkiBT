using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class WrapFieldResolver<T> : FieldResolver<WrapField<T>, T>
    {
        public WrapFieldResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        private WrapField<T> editorField;
        protected override WrapField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new WrapField<T>(fieldInfo.Name);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo fieldInfo)
        {
            return infoType == typeof(T) && fieldInfo.GetCustomAttribute<WrapFieldAttribute>() != null;
        }
    }
    public class WrapField<T> : BaseField<T>
    {
        GenericObjectWrapper<T> m_Instance;
        private GenericObjectWrapper<T> Instance => m_Instance != null ? m_Instance : GetInstance();
        private SerializedObject m_SerializedObject;
        private SerializedProperty m_SerializedProperty;
        public WrapField(string label) : base(label, null)
        {
            var element = CreateView();
            Add(element);
        }
        private GenericObjectWrapper<T> GetInstance()
        {
            m_Instance = GenericObjectWrapperHelper.Wrap<T>() as GenericObjectWrapper<T>;
            m_Instance.Value = value;
            m_SerializedObject = new SerializedObject(m_Instance);
            m_SerializedProperty = m_SerializedObject.FindProperty("m_Value");
            return m_Instance;
        }
        VisualElement CreateView()
        {
            return new IMGUIContainer(() =>
            {
                m_SerializedObject.Update();
                EditorGUILayout.PropertyField(m_SerializedProperty);
                if (m_SerializedObject.ApplyModifiedProperties())
                {
                    ChangeValueWithNotify(base.value, Instance.Value);
                }
            });
        }
        private void ChangeValueWithNotify(T oldValue, T newValue)
        {
            base.value = newValue;
            using ChangeEvent<T> changeEvent = ChangeEvent<T>.GetPooled(oldValue, newValue);
            changeEvent.target = this;
            SendEvent(changeEvent);
        }
        public sealed override T value
        {
            get => base.value;
            set
            {
                if (value == null)
                {
                    Instance.Value = (T)Activator.CreateInstance(typeof(T));
                }
                else
                {
                    Instance.Value = ReflectionHelper.DeepCopy(value);
                }
                ChangeValueWithNotify(base.value, Instance.Value);
            }
        }
    }
}
