using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class UnityEventResolver : FieldResolver<UnityEventField, UnityEvent>
    {
        public UnityEventResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        private UnityEventField editorField;
        protected override UnityEventField CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new UnityEventField(fieldInfo.Name, null);
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _) => infoType == typeof(UnityEvent);

    }
    public class UnityEventField : BaseField<UnityEvent>
    {
        UnityEventInstance m_Instance;
        private UnityEventInstance Instance => m_Instance != null ? m_Instance : GetInstance();
        SerializedObject m_SerializedObject;
        SerializedProperty m_SerializedProperty;
        public UnityEventField(string label, VisualElement visualInput) : base(label, visualInput)
        {
            var element = CreateUnityEventNode();
            Add(element);
        }
        private UnityEventInstance GetInstance()
        {
            m_Instance = ScriptableObject.CreateInstance<UnityEventInstance>();
            m_Instance.unityEvent = value;
            m_SerializedObject = new SerializedObject(m_Instance);
            m_SerializedProperty = m_SerializedObject.FindProperty("unityEvent");
            return m_Instance;
        }
        VisualElement CreateUnityEventNode()
        {
            return new IMGUIContainer(() =>
            {
                m_SerializedObject.Update();
                EditorGUILayout.PropertyField(m_SerializedProperty);
                if (m_SerializedObject.ApplyModifiedProperties())
                {
                    base.value = Instance.unityEvent;
                }
            });
        }
        public sealed override UnityEvent value
        {
            get => base.value;
            set
            {
                if (value == null)
                {
                    Instance.unityEvent = new();
                }
                else
                {
                    Instance.unityEvent = ReflectionHelper.DeepCopy(value);
                }
                base.value = Instance.unityEvent;
            }
        }
    }
    public class UnityEventInstance : ScriptableObject
    {
        public UnityEvent unityEvent;
    }
}
