using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public static class BehaviorNodeExtension
    {
        public static T GetSharedVariableValue<T>(this IBehaviorTreeNode node, string fieldName)
        {
            var sharedVariable = node.GetSharedVariable<SharedVariable<T>>(fieldName);
            return sharedVariable != null ? node.MapTreeView.GetSharedVariableValue(sharedVariable) : default;
        }
        public static T GetSharedVariable<T>(this IBehaviorTreeNode node, string fieldName) where T : SharedVariable
        {
            try
            {
                return (T)node.GetFieldResolver(fieldName).Value;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
        public static T GetSharedVariableValue<T>(this ITreeView treeView, SharedVariable<T> variable)
        {
            if (variable.IsShared)
            {
                if (!treeView.TryGetExposedProperty(variable.Name, out SharedVariable<T> mapContent)) return variable.Value;
                return mapContent.Value;
            }
            else
            {
                return variable.Value;
            }
        }
        public static bool TryGetExposedProperty<T>(this ITreeView treeView, string name, out T variable) where T : SharedVariable
        {
            variable = (T)treeView.SharedVariables.Where(x => x is T && x.Name.Equals(name)).FirstOrDefault();
            return variable != null;
        }
        public static GroupBlock CreateBlock(this ITreeView treeView, Rect rect, GroupBlockData blockData = null)
        {
            blockData ??= new GroupBlockData();
            var group = new GroupBlock
            {
                autoUpdateGeometry = true,
                title = blockData.Title
            };
            treeView.View.AddElement(group);
            group.SetPosition(rect);
            return group;
        }
        public static void SelectGroup(this ITreeView treeView, IBehaviorTreeNode node)
        {
            var block = treeView.CreateBlock(new Rect((node as Node).transform.position, new Vector2(100, 100)));
            foreach (var select in treeView.View.selection)
            {
                if (select is not IBehaviorTreeNode || select is RootNode) continue;
                block.AddElement(select as Node);
            }
        }
        public static void UnselectGroup(this ITreeView treeView)
        {
            foreach (var select in treeView.View.selection)
            {
                if (select is not IBehaviorTreeNode) continue;
                var node = select as Node;
                var block = treeView.View.graphElements.OfType<GroupBlock>().FirstOrDefault(x => x.ContainsElement(node));
                block?.RemoveElement(node);
            }
        }
    }
}