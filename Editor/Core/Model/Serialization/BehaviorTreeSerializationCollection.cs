using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    [Serializable]
    public class BehaviorTreeSerializationPair
    {
        public BehaviorTreeSO behaviorTreeSO;
        public TextAsset serializedData;
        public BehaviorTreeSerializationPair(BehaviorTreeSO behaviorTreeSO, TextAsset serializedData)
        {
            this.behaviorTreeSO = behaviorTreeSO;
            this.serializedData = serializedData;
        }
    }
    [Serializable]
    public class BehaviorTreeSerializationCollection
    {
        public List<BehaviorTreeSerializationPair> serializationPairs;
        public List<string> guids;
        public void SetUp()
        {
            HashSet<TextAsset> serializedDataSet;
            if (serializationPairs != null)
                serializedDataSet = serializationPairs.Select(x => x.serializedData).Where(x => x != null).ToHashSet();
            else
                serializedDataSet = new();
            serializationPairs = new();
            guids = AssetDatabase.FindAssets($"t:{typeof(BehaviorTreeSO)}").ToList();
            var list = guids.Select(x => AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>(AssetDatabase.GUIDToAssetPath(x))).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var so = list[i];
                serializationPairs.Add(new BehaviorTreeSerializationPair(
                    so,
                    serializedDataSet.FirstOrDefault(x => x.name == $"{so.name}_{guids[i]}")
                ));
            }
        }
        public void InjectJsonFiles(HashSet<TextAsset> dataSet)
        {
            for (int i = 0; i < serializationPairs.Count; i++)
            {
                serializationPairs[i].serializedData = dataSet.FirstOrDefault(x => x.name == $"{serializationPairs[i].behaviorTreeSO.name}_{guids[i]}");
            }
        }
        public BehaviorTreeSerializationPair FindSerializationPair(BehaviorTreeSO behaviorTreeSO)
        {
            return serializationPairs
            .FirstOrDefault(x => x.behaviorTreeSO == behaviorTreeSO);
        }
    }
}
