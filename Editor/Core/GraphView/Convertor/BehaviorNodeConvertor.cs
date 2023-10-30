using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorNodeConvertor
    {
        private interface IParentAdapter
        {
            void Connect(ITreeView treeView, IBehaviorTreeNode nodeToConnect);
        }
        private readonly struct PortAdapter : IParentAdapter
        {
            private readonly Port port;
            public PortAdapter(Port port)
            {
                this.port = port;
            }
            public void Connect(ITreeView treeView, IBehaviorTreeNode nodeToConnect)
            {
                var edge = PortHelper.ConnectPorts(port, nodeToConnect.Parent);
                (treeView as GraphView).Add(edge);
            }
        }
        private readonly struct StackAdapter : IParentAdapter
        {
            private readonly StackNode stackNode;
            public StackAdapter(StackNode stackNode)
            {
                this.stackNode = stackNode;
            }
            public void Connect(ITreeView treeView, IBehaviorTreeNode nodeToConnect)
            {
                if (nodeToConnect is BehaviorTreeNode behaviorTreeNode)
                {
                    stackNode.AddElement(behaviorTreeNode);
                    behaviorTreeNode.Parent.SetEnabled(false);
                }
            }
        }
        private readonly struct EdgePair
        {
            public readonly NodeBehavior NodeBehavior;
            public readonly IParentAdapter Adapter;

            public EdgePair(NodeBehavior nodeBehavior, IParentAdapter adapter)
            {
                NodeBehavior = nodeBehavior;
                Adapter = adapter;
            }
        }
        private readonly NodeResolverFactory nodeResolver = NodeResolverFactory.Instance;
        private readonly List<IBehaviorTreeNode> tempNodes = new();
        public (RootNode, IEnumerable<IBehaviorTreeNode>) ConvertToNode<T>(IBehaviorTree tree, T treeView, Vector2 initPos) where T : GraphView, ITreeView
        {
            var stack = new Stack<EdgePair>();
            RootNode root = null;
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
                var node = nodeResolver.Create(edgePair.NodeBehavior.GetType(), treeView);
                node.Restore(edgePair.NodeBehavior);
                treeView.AddElement(node.View);
                tempNodes.Add(node);
                var rect = edgePair.NodeBehavior.graphPosition;
                rect.position += initPos;
                node.View.SetPosition(rect);

                // connect parent
                edgePair.Adapter?.Connect(treeView, node);

                // seek child
                switch (edgePair.NodeBehavior)
                {
                    case Composite nb:
                        {
                            if (node is CompositeNode compositeNode)
                            {
                                var addible = nb.Children.Count - compositeNode.ChildPorts.Count;
                                if (compositeNode.NoValidate && nb.Children.Count == 0)
                                {
                                    compositeNode.RemoveUnnecessaryChildren();
                                    break;
                                }
                                for (var i = 0; i < addible; i++)
                                {
                                    compositeNode.AddChild();
                                }
                                for (var i = 0; i < nb.Children.Count; i++)
                                {
                                    stack.Push(new EdgePair(nb.Children[i], new PortAdapter(compositeNode.ChildPorts[i])));
                                }
                            }
                            else if (node is CompositeStack stackNode)
                            {
                                for (var i = nb.Children.Count - 1; i >= 0; i--)
                                {
                                    stack.Push(new EdgePair(nb.Children[i], new StackAdapter(stackNode)));
                                }
                            }
                            break;
                        }
                    case Conditional nb:
                        {
                            var conditionalNode = node as ConditionalNode;
                            stack.Push(new EdgePair(nb.Child, new PortAdapter(conditionalNode.Child)));
                            break;
                        }
                    case Decorator nb:
                        {
                            var decoratorNode = node as DecoratorNode;
                            stack.Push(new EdgePair(nb.Child, new PortAdapter(decoratorNode.Child)));
                            break;
                        }
                    case Root nb:
                        {
                            root = node as RootNode;
                            if (nb.Child != null)
                            {
                                stack.Push(new EdgePair(nb.Child, new PortAdapter(root.Child)));
                            }
                            break;
                        }
                }
            }
            return (root, tempNodes);
        }
    }
}
