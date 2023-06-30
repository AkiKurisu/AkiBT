using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorNodeConverter
    {
        private readonly struct EdgePair
        {
            public readonly NodeBehavior NodeBehavior;
            public readonly Port ParentOutputPort;

            public EdgePair(NodeBehavior nodeBehavior, Port parentOutputPort)
            {
                NodeBehavior = nodeBehavior;
                ParentOutputPort = parentOutputPort;
            }
        }
        private readonly NodeResolver nodeResolver = new NodeResolver();
        private List<BehaviorTreeNode> tempNodes=new List<BehaviorTreeNode>();
        public (RootNode,IEnumerable<BehaviorTreeNode>) ConvertToNode<T>(IBehaviorTree tree,T treeView,Vector2 initPos)where T:GraphView,ITreeView
        {
            var stack = new Stack<EdgePair>();
            RootNode root=null;
            stack.Push(new EdgePair(tree.Root, null));
            tempNodes.Clear();
            while (stack.Count > 0)
            {
                // create node
                var edgePair = stack.Pop();
                if (edgePair.NodeBehavior == null)
                {
                    continue;
                }
                var node = nodeResolver.CreateNodeInstance(edgePair.NodeBehavior.GetType(),treeView);
                node.Restore(edgePair.NodeBehavior);
                treeView.AddElement(node);
                tempNodes.Add(node);
                var rect=edgePair.NodeBehavior.graphPosition;
                rect.position+=initPos;
                node.SetPosition(rect);

                // connect parent
                if (edgePair.ParentOutputPort != null)
                {
                    var edge = BehaviorTreeView.ConnectPorts(edgePair.ParentOutputPort, node.Parent);
                    treeView.Add(edge);
                }

                // seek child
                switch (edgePair.NodeBehavior)
                {
                    case Composite nb:
                    {
                        var compositeNode = node as CompositeNode;
                        var addible = nb.Children.Count - compositeNode.ChildPorts.Count;
                        for (var i = 0; i < addible; i++)
                        {
                            compositeNode.AddChild();
                        }

                        for (var i = 0; i < nb.Children.Count; i++)
                        {
                            stack.Push(new EdgePair(nb.Children[i], compositeNode.ChildPorts[i]));
                        }
                        break;
                    }
                    case Conditional nb:
                    {
                        var conditionalNode = node as ConditionalNode;
                        stack.Push(new EdgePair(nb.Child, conditionalNode.Child));
                        break;
                    }
                    case Decorator nb:
                    {
                        var decoratorNode = node as DecoratorNode;
                        stack.Push(new EdgePair(nb.Child, decoratorNode.Child));
                        break;
                    }
                    case Root nb:
                    {
                        root = node as RootNode;
                        if (nb.Child != null)
                        {
                            stack.Push(new EdgePair(nb.Child, root.Child));
                        }
                        break;
                    }
                }
            }
            return (root,tempNodes);
        }
    }
}
