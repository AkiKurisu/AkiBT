using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class NodeResolver
    {
        private StyleSheet styleSheetCache;
        public BehaviorTreeNode CreateNodeInstance(Type type,ITreeView treeView)
        {
            BehaviorTreeNode node;
            if (type.IsSubclassOf(typeof(Composite)))
            {
                node = new CompositeNode();
            } else if (type.IsSubclassOf(typeof(Conditional)))
            {
                node = new ConditionalNode();
            } 
            else if (type.IsSubclassOf(typeof(Decorator)))
            {
                node = new DecoratorNode();
            } 
            else if (type == typeof(Root))
            {
                node = new RootNode();
            }
            else
            {
                node = new ActionNode();
            }
            node.SetBehavior(type,treeView);
            if(styleSheetCache==null)styleSheetCache=BehaviorTreeSetting.GetNodeStyle(treeView.treeEditorName);
            node.styleSheets.Add(styleSheetCache);
            return node;
        }
    }
}