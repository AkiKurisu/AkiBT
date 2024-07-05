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
        public BehaviorTreeAsset behaviorTreeAsset;
        public TextAsset serializedData;
        public BehaviorTreeSerializationPair(BehaviorTreeAsset behaviorTreeAsset, TextAsset serializedData)
        {
            this.behaviorTreeAsset = behaviorTreeAsset;
            this.serializedData = serializedData;
        }
    }
    [Serializable]
    public class BehaviorTreeSerializationCollection
    {
        public BehaviorTreeSerializationPair[] serializationPairs;
        public string[] guids;
        public void SetUp()
        {
            HashSet<TextAsset> serializedDataSet;
            if (serializationPairs != null)
                serializedDataSet = serializationPairs.Select(x => x.serializedData).Where(x => x != null).ToHashSet();
            else
                serializedDataSet = new();
            guids = BehaviorTreeSearchUtility.GetAllBehaviorTreeAssetGuids();
            serializationPairs = BehaviorTreeSearchUtility.GetBehaviorTreeAssets(guids).Select((x, id) =>
            {
                return new BehaviorTreeSerializationPair(
                    x,
                    serializedDataSet.FirstOrDefault(x => x.name == $"{x.name}_{guids[id]}")
                );
            }).ToArray();
        }
        public void InjectJsonFiles(HashSet<TextAsset> dataSet)
        {
            for (int i = 0; i < serializationPairs.Length; i++)
            {
                serializationPairs[i].serializedData = dataSet.FirstOrDefault(x => x.name == $"{serializationPairs[i].behaviorTreeAsset.name}_{guids[i]}");
            }
        }
        public BehaviorTreeSerializationPair FindSerializationPair(BehaviorTreeAsset behaviorTreeContainerSo)
        {
            return serializationPairs
            .FirstOrDefault(x => x.behaviorTreeAsset == behaviorTreeContainerSo);
        }
    }
}
