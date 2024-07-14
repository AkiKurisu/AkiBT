using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    [CustomPropertyDrawer(typeof(APIUpdateConfig.SerializeType))]
    public class SerializeTypeDrawer : PropertyDrawer
    {
        private const string NullType = "Null";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var reference = property.FindPropertyRelative("nodeType");
            if (EditorGUI.DropdownButton(position, new GUIContent(reference.stringValue), FocusType.Keyboard))
            {
                var provider = ScriptableObject.CreateInstance<NodeTypeSearchWindow>();
                provider.Initialize((type) =>
                {
                    var serializeType = new APIUpdateConfig.SerializeType(type);
                    reference.stringValue = serializeType != null ? serializeType.nodeType : NullType;
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
