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
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType, BehaviorTreeUserServiceData serviceData, List<BehaviorTreeSO> searchList)
        {
            if (serviceData == null) serviceData = BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
            if (searchList == null) searchList = GetAllBehaviorTreeSO();
            Stack<NodeBehavior> stack = new Stack<NodeBehavior>();
            List<BehaviorTreeSO> behaviorTreeSOs = new();
            List<BehaviorTreeSerializationPair> pairs = new();
            foreach (var treeSO in searchList)
            {
                SearchBehavior(treeSO, stack, searchType, behaviorTreeSOs);
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
        private static void SearchBehavior(BehaviorTreeSO treeSO, Stack<NodeBehavior> stack, Type checkType, List<BehaviorTreeSO> behaviorTreeSOs)
        {
            if (checkType == null)
            {
                behaviorTreeSOs.Add(treeSO);
                return;
            }
            stack.Clear();
            stack.Push(treeSO.Root);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (node == null) return;
                if (node.GetType() == checkType)
                {
                    behaviorTreeSOs.Add(treeSO);
                    return;
                }
                if (node is Root)
                {
                    stack.Push((node as Root).Child);
                    continue;
                }
                if (node is Conditional)
                {
                    var child = (node as Conditional).Child;
                    if (child != null) stack.Push(child);
                    continue;
                }
                if (node is Decorator)
                {
                    stack.Push((node as Decorator).Child);
                    continue;
                }
                if (node is Composite)
                {
                    foreach (var child in (node as Composite).Children) stack.Push(child);
                    continue;
                }
            }
        }
    }
}
