using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeSearchUtility : MonoBehaviour
    {
        public static List<BehaviorTreeSerializationPair> SearchBehaviorTreeSO(Type searchType)
        {
            var serviceData=BehaviorTreeSetting.GetOrCreateSettings().ServiceData;
            var guids=AssetDatabase.FindAssets($"t:{typeof(BehaviorTreeSO)}");
            Stack<NodeBehavior> stack=new Stack<NodeBehavior>();
            List<BehaviorTreeSO> behaviorTreeSOs=new();
            List<BehaviorTreeSerializationPair> pairs=new();
            foreach(var guid in guids)
            { 
                var treeSO=AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>(AssetDatabase.GUIDToAssetPath(guid));
                SearchBehavior(treeSO,stack,searchType,behaviorTreeSOs);
            }
            foreach(var so in behaviorTreeSOs)
            {
                pairs.Add(new BehaviorTreeSerializationPair(){behaviorTreeSO=so,serializedData=serviceData.serializationCollection.FindSerializeData(so)});
            }
            return pairs;
        }
        private static void SearchBehavior(BehaviorTreeSO treeSO,Stack<NodeBehavior> stack,Type checkType,List<BehaviorTreeSO> behaviorTreeSOs)
        {
            stack.Clear();
            stack.Push(treeSO.Root);
            while(stack.Count>0)
            {
                var node = stack.Pop();
                if(node==null)return;
                if(node.GetType()==checkType)
                {
                    behaviorTreeSOs.Add(treeSO);
                    return;
                }
                if(node is Root)
                {
                    stack.Push((node as Root).Child);
                    continue;
                }
                if(node is Conditional)
                {
                    var child=(node as Conditional).Child;
                    if(child!=null)stack.Push(child);
                    continue;
                }
                if(node is Decorator)
                {
                    stack.Push((node as Decorator).Child);
                    continue;
                }
                if(node is Composite)
                {
                    foreach(var child in (node as Composite).Children)stack.Push(child);
                    continue;
                }
            }
        }
    }
}
