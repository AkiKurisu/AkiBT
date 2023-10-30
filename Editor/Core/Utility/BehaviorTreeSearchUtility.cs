using System;
using System.Collections.Generic;
using UnityEditor;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeSearchUtility
    {
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType)
        {
            return SearchBehaviorTreeSO(searchType, BehaviorTreeSetting.GetOrCreateSettings().ServiceData, GetAllBehaviorTreeSO());
        }
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType, BehaviorTreeServiceData serviceData, List<BehaviorTreeSO> searchList)
        {
            if (serviceData == null) serviceData = BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
            searchList ??= GetAllBehaviorTreeSO();
            List<BehaviorTreeSO> behaviorTreeSOs = new();
            List<BehaviorTreeSerializationPair> pairs = new();
            foreach (var treeSO in searchList)
            {
                SearchBehavior(treeSO, searchType, behaviorTreeSOs);
            }
            foreach (var so in behaviorTreeSOs)
            {
                var pair = serviceData.serializationCollection.FindSerializationPair(so);
                pairs.Add(new BehaviorTreeSerializationPair(so, pair.serializedData));
            }
            return pairs;
        }
        public static List<BehaviorTreeSO> GetAllBehaviorTreeSO()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(BehaviorTreeSO)}");
            List<BehaviorTreeSO> behaviorTreeSOs = new();
            foreach (var guid in guids)
            {
                behaviorTreeSOs.Add(AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            return behaviorTreeSOs;
        }
        private static void SearchBehavior(BehaviorTreeSO treeSO, Type checkType, List<BehaviorTreeSO> behaviorTreeSOs)
        {
            if (checkType == null)
            {
                behaviorTreeSOs.Add(treeSO);
                return;
            }
            foreach (var node in treeSO.Traverse())
            {
                if (node.GetType() == checkType)
                {
                    behaviorTreeSOs.Add(treeSO);
                    return;
                }
            }
        }
    }
}
