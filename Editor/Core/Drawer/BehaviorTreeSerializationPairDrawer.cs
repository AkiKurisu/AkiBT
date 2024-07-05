using System.IO;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    [CustomPropertyDrawer(typeof(BehaviorTreeSerializationPair))]
    public class BehaviorTreeSerializationPairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var soProperty = property.FindPropertyRelative("behaviorTreeAsset");
            var textProperty = property.FindPropertyRelative("serializedData");
            position.width /= 3;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, soProperty, GUIContent.none);
            GUI.enabled = true;
            position.x += position.width;
            EditorGUI.PropertyField(position, textProperty, GUIContent.none);
            position.x += position.width;
            position.width /= 2;
            GUI.enabled = textProperty.objectReferenceValue != null;
            if (GUI.Button(position, "Serialize"))
            {
                var asset = soProperty.objectReferenceValue as BehaviorTreeAsset;
                var serializedData = BehaviorTree.Serialize(asset.GetBehaviorTree(), true, true);
                var textPath = Application.dataPath + AssetDatabase.GetAssetPath(textProperty.objectReferenceValue).Replace("Assets", string.Empty);
                File.WriteAllText(textPath, serializedData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            position.x += position.width;
            if (GUI.Button(position, "Deserialize"))
            {
                var asset = soProperty.objectReferenceValue as BehaviorTreeAsset;
                Undo.RecordObject(asset, "Deserialize from json file");
                var data = BehaviorTreeData.Deserialize((textProperty.objectReferenceValue as TextAsset).text);
                asset.SetBehaviorTreeData(data);
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.enabled = true;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var soProperty = property.FindPropertyRelative("behaviorTreeSO");
            return base.GetPropertyHeight(soProperty, label);
        }
    }
}
