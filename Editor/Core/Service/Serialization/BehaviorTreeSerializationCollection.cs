using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using System.IO;
namespace Kurisu.AkiBT.Editor
{
    [Serializable]
    public class BehaviorTreeSerializationPair
    {
        public BehaviorTreeSO behaviorTreeSO;
        public TextAsset serializedData;
    }
    [Serializable]
    public class BehaviorTreeSerializationCollection
    {
        public List<BehaviorTreeSerializationPair> serializationPairs;
        public List<string> guids;
        public void SetUp()
        {
            HashSet<TextAsset> serializedDataSet;
            if(serializationPairs!=null)
                serializedDataSet=serializationPairs.Select(x=>x.serializedData).Where(x=>x!=null).ToHashSet();
            else
                serializedDataSet=new();
            serializationPairs=new();
            guids=AssetDatabase.FindAssets($"t:{typeof(BehaviorTreeSO)}").ToList();
            var list = guids.Select(x=>AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>(AssetDatabase.GUIDToAssetPath(x))).ToList();
            for(int i=0;i<list.Count;i++)
            {
                var so=list[i];
                serializationPairs.Add(new BehaviorTreeSerializationPair(){behaviorTreeSO=so,serializedData=serializedDataSet.FirstOrDefault(x=>x.name==$"{so.name}_{guids[i]}")});
            }
        }
        public void InjectJsonFiles(HashSet<TextAsset> dataSet)
        {
            for(int i=0;i<serializationPairs.Count;i++)
            {
                serializationPairs[i].serializedData=dataSet.FirstOrDefault(x=>x.name==$"{serializationPairs[i].behaviorTreeSO.name}_{guids[i]}");
            }
        }
        public TextAsset FindSerializeData(BehaviorTreeSO behaviorTreeSO)
        {
            return serializationPairs
            .Select(x=>x.serializedData)
            .Where(x=>x!=null)
            .ToHashSet()
            .FirstOrDefault(x=>x.name==$"{behaviorTreeSO.name}_{AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(behaviorTreeSO))}");
        }
    }
    [CustomPropertyDrawer(typeof(BehaviorTreeSerializationPair))]
    public class BehaviorTreeSerializationPairDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var soProperty=property.FindPropertyRelative("behaviorTreeSO");
            var textProperty=property.FindPropertyRelative("serializedData");
            position.width/=3;
            GUI.enabled=false;
            EditorGUI.PropertyField(position,soProperty,GUIContent.none);
            GUI.enabled=true;
            position.x+=position.width;
            EditorGUI.PropertyField(position,textProperty,GUIContent.none);
            position.x+=position.width;
            position.width/=2;
            GUI.enabled=textProperty.objectReferenceValue!=null;
            if(GUI.Button(position,"Serialize"))
            {
                var serializedData=BehaviorTreeSerializeUtility.SerializeTree(soProperty.objectReferenceValue as BehaviorTreeSO,true,true);
                var textPath=Application.dataPath+AssetDatabase.GetAssetPath(textProperty.objectReferenceValue).Replace("Assets",string.Empty);
                File.WriteAllText(textPath,serializedData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            position.x+=position.width;
            if(GUI.Button(position,"Deserialize"))
            {
                var treeSO=soProperty.objectReferenceValue as BehaviorTreeSO;
                Undo.RecordObject(treeSO,"Deserialize from json file");
                treeSO.Deserialize((textProperty.objectReferenceValue as TextAsset).text);
                EditorUtility.SetDirty(treeSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUI.enabled=true;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var soProperty=property.FindPropertyRelative("behaviorTreeSO");
            return base.GetPropertyHeight(soProperty, label);
        }
    }
}
