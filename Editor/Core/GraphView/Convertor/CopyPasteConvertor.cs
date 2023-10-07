using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class CopyPasteConvertor
    {
        private readonly ITreeView sourceView;
        private readonly List<ISelectable> pasteElements;
        private readonly Dictionary<Port, Port> portCopyDict;
        private readonly Dictionary<IBehaviorTreeNode, IBehaviorTreeNode> nodeCopyDict;
        private readonly List<ISelectable> copyElements;
        public CopyPasteConvertor(ITreeView sourceView, List<ISelectable> copyElements)
        {
            this.sourceView = sourceView;
            this.copyElements = copyElements;
            pasteElements = new List<ISelectable>();
            portCopyDict = new Dictionary<Port, Port>();
            nodeCopyDict = new Dictionary<IBehaviorTreeNode, IBehaviorTreeNode>();
            DistinctNodes();
            CopyNodes();
            CopyEdges();
            CopyGroupBlocks();
        }
        public List<ISelectable> GetPasteElements() => pasteElements;
        private void DistinctNodes()
        {
            var containerNodes = copyElements.OfType<CompositeStack>().ToArray();
            foreach (var containerNode in containerNodes)
            {
                containerNode.contentContainer.Query<BehaviorTreeNode>().ForEach(x => copyElements.Remove(x));
            }
        }
        private void CopyNodes()
        {
            foreach (var select in copyElements)
            {
                if (select is not IBehaviorTreeNode selectNode) continue;
                var node = sourceView.DuplicateNode(selectNode);
                pasteElements.Add(node.View);
                nodeCopyDict.Add(selectNode, node);
                CopyPort(selectNode, node);
            }
        }
        private void CopyPort(IBehaviorTreeNode selectNode, IBehaviorTreeNode node)
        {
            if (selectNode is CompositeStack compositeStack)
            {
                var copyMap = compositeStack.GetCopyMap();
                compositeStack.contentContainer
                .Query<Node>()
                .ToList()
                .OfType<IChildPortable>()
                .ToList()
                .ForEach(x =>
                {
                    portCopyDict.Add(x.Child, copyMap[x.GetHashCode()].Child);
                });
            }
            else if (selectNode is ActionNode actionNode)
            {
                portCopyDict.Add(actionNode.Parent, (node as ActionNode).Parent);
            }
            else if (selectNode is CompositeNode compositeNode)
            {
                var copy = node as CompositeNode;
                int count = compositeNode.ChildPorts.Count - copy.ChildPorts.Count;
                for (int i = 0; i < count; i++)
                {
                    copy.AddChild();
                }
                for (int i = 0; i < compositeNode.ChildPorts.Count; i++)
                {
                    portCopyDict.Add(compositeNode.ChildPorts[i], copy.ChildPorts[i]);
                }
                portCopyDict.Add(compositeNode.Parent, copy.Parent);
            }
            else if (selectNode is ConditionalNode conditionalNode)
            {
                portCopyDict.Add(conditionalNode.Child, (node as ConditionalNode).Child);
                portCopyDict.Add(conditionalNode.Parent, (node as ConditionalNode).Parent);

            }
            else if (selectNode is DecoratorNode decoratorNode)
            {
                portCopyDict.Add(decoratorNode.Child, (node as DecoratorNode).Child);
                portCopyDict.Add(decoratorNode.Parent, (node as DecoratorNode).Parent);
            }
        }

        private void CopyEdges()
        {
            foreach (var select in copyElements)
            {
                if (select is not Edge edge) continue;
                if (!portCopyDict.ContainsKey(edge.input) || !portCopyDict.ContainsKey(edge.output)) continue;
                var newEdge = PortHelper.ConnectPorts(portCopyDict[edge.output], portCopyDict[edge.input]);
                sourceView.View.AddElement(newEdge);
                pasteElements.Add(newEdge);
            }
        }
        private void CopyGroupBlocks()
        {
            foreach (var select in copyElements)
            {
                if (select is not GroupBlock selectBlock) continue;
                var nodes = selectBlock.containedElements.OfType<IBehaviorTreeNode>();
                Rect newRect = selectBlock.GetPosition();
                newRect.position += new Vector2(50, 50);
                var block = sourceView.GroupBlockController.CreateBlock(newRect);
                block.title = selectBlock.title;
                block.AddElements(nodes.Where(x => nodeCopyDict.ContainsKey(x)).Select(x => nodeCopyDict[x].View));
            }
        }
    }
}
