using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    [CreateAssetMenu(fileName = "APIUpdateConfig", menuName = "AkiBT/APIUpdateConfig")]
    public class APIUpdateConfig : ScriptableObject
    {
        [Serializable]
        public class SerializeType
        {
            private Type type;
            public Type Type => type ??= Type.GetType(assemblyQualifiedName);
            public string assemblyQualifiedName;
            public string GetFullTypeName()
            {
                return $"{Type.Assembly.GetName().Name} {Type.FullName}";
            }
            public SerializeType() { }
            public SerializeType(Type type)
            {
                assemblyQualifiedName = type.AssemblyQualifiedName;
            }
        }
        [Serializable]
        public class Pair
        {
            public SerializeType sourceType;
            public SerializeType targetType;
            public Pair() { }
            public Pair(Type sourceType, Type targetType)
            {
                this.sourceType = new SerializeType(sourceType);
                this.targetType = new SerializeType(targetType);
            }
        }
        [field: SerializeField]
        public Pair[] Pairs { get; set; }
    }
    [CustomPropertyDrawer(typeof(APIUpdateConfig.SerializeType))]
    public class SerializeTypeDrawer : PropertyDrawer
    {
        private const string NullType = "Null";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var reference = property.FindPropertyRelative("assemblyQualifiedName");
            var type = Type.GetType(reference.stringValue);
            string id = type != null ? $"{type.Assembly.GetName().Name} {type.Namespace} {type.Name}" : NullType;
            if (EditorGUI.DropdownButton(position, new GUIContent(id), FocusType.Keyboard))
            {
                var provider = ScriptableObject.CreateInstance<NodeTypeSearchWindow>();
                provider.Initialize((type) =>
                {
                    reference.stringValue = type?.AssemblyQualifiedName ?? NullType;
                    property.serializedObject.ApplyModifiedProperties();
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
            EditorGUI.EndProperty();
        }
    }
    [CustomEditor(typeof(APIUpdateConfig))]
    public class APIUpdateConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update API"))
            {
                APIUpdater.UpdateAPI(typeof(BehaviorTreeAsset), target as APIUpdateConfig);
            }
        }
    }
}
