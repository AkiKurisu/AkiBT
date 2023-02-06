using System;

namespace Kurisu.AkiBT.Editor
{
    public class NodeResolver
    {
        public BehaviorTreeNode CreateNodeInstance(Type type,BehaviorTreeView treeView)
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
            return node;
        }
    }
}