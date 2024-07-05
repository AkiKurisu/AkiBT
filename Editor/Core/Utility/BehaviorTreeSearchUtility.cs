using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeSearchUtility
    {
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType)
        {
            return SearchBehaviorTreeSO(searchType, BehaviorTreeSetting.GetOrCreateSettings().ServiceData, GetAllBehaviorTreeAssets());
        }
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType, BehaviorTreeServiceData serviceData, List<BehaviorTreeAsset> searchList)
        {
            if (serviceData == null) serviceData = BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
            searchList ??= GetAllBehaviorTreeAssets();
            List<BehaviorTreeAsset> behaviorTreeAssets = new();
            List<BehaviorTreeSerializationPair> pairs = new();
            foreach (var treeSO in searchList)
            {
                SearchBehavior(treeSO, searchType, behaviorTreeAssets);
            }
            foreach (var so in behaviorTreeAssets)
            {
                var pair = serviceData.serializationCollection.FindSerializationPair(so);
                pairs.Add(new BehaviorTreeSerializationPair(so, pair.serializedData));
            }
            return pairs;
        }
        public static string[] GetAllBehaviorTreeAssetGuids()
        {
            return AssetDatabase.FindAssets($"t:{typeof(BehaviorTreeAsset)}");
        }
        public static List<BehaviorTreeAsset> GetAllBehaviorTreeAssets()
        {
            var guids = GetAllBehaviorTreeAssetGuids();
            return GetBehaviorTreeAssets(guids);
        }
        public static List<BehaviorTreeAsset> GetBehaviorTreeAssets(string[] guids)
        {
            return guids.Select(x => AssetDatabase.LoadAssetAtPath<BehaviorTreeAsset>(AssetDatabase.GUIDToAssetPath(x))).ToList();
        }
        private static void SearchBehavior(BehaviorTreeAsset btAsset, Type checkType, List<BehaviorTreeAsset> behaviorTreeSOs)
        {
            if (checkType == null)
            {
                behaviorTreeSOs.Add(btAsset);
                return;
            }
            foreach (var node in btAsset.GetBehaviorTree())
            {
                if (node.GetType() == checkType)
                {
                    behaviorTreeSOs.Add(btAsset);
                    return;
                }
            }
        }
    }
}
