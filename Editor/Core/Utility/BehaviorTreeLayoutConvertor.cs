using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeLayoutConvertor : INodeForLayoutConvertor
    {
        public float SiblingDistance { get; private set; }
        private NodeAutoLayouter.TreeNode m_LayoutRootNode;
        public NodeAutoLayouter.TreeNode LayoutRootNode => m_LayoutRootNode;
        public IBinaryTreeNode PrimRootNode => m_PrimRootNode;
        private IBinaryTreeNode m_PrimRootNode;
        private const NodeAutoLayouter.CalculateMode CalculateMode = NodeAutoLayouter.CalculateMode.Horizontal | NodeAutoLayouter.CalculateMode.Positive;
        public INodeForLayoutConvertor Init(IBinaryTreeNode primRootNode)
        {
            m_PrimRootNode = primRootNode;
            SiblingDistance = BehaviorTreeSetting.GetOrCreateSettings().AutoLayoutSiblingDistance;
            return this;
        }
        public NodeAutoLayouter.TreeNode PrimNode2LayoutNode()
        {
            if (m_PrimRootNode.View.layout.width == float.NaN)
            {
                return null;
            }

            m_LayoutRootNode =
                new NodeAutoLayouter.TreeNode(m_PrimRootNode.View.layout.height + SiblingDistance,
                    m_PrimRootNode.View.layout.width,
                    m_PrimRootNode.View.layout.y,
                    CalculateMode);

            Convert2LayoutNode(m_PrimRootNode,
                m_LayoutRootNode, m_PrimRootNode.View.layout.y + m_PrimRootNode.View.layout.width,
                CalculateMode);
            return m_LayoutRootNode;
        }

        private void Convert2LayoutNode(IBinaryTreeNode rootPrimNode,
            NodeAutoLayouter.TreeNode rootLayoutNode, float lastHeightPoint,
            NodeAutoLayouter.CalculateMode calculateMode)
        {
            foreach (var childNode in rootPrimNode.GetBinaryTreeChildren())
            {
                NodeAutoLayouter.TreeNode childLayoutNode =
                    new(childNode.View.layout.height + SiblingDistance, childNode.View.layout.width,
                        lastHeightPoint + SiblingDistance,
                        calculateMode);
                rootLayoutNode.AddChild(childLayoutNode);
                Convert2LayoutNode(childNode, childLayoutNode,
                    lastHeightPoint + SiblingDistance + childNode.View.layout.width, calculateMode);
            }
        }

        public void LayoutNode2PrimNode()
        {
            var rootNode = m_PrimRootNode.GetBinaryTreeChildren()[0];
            var offSet = (rootNode.View as GraphElement).contentRect.position - m_LayoutRootNode.children[0].GetPos() + new Vector2(400, 300);
            Convert2PrimNode(rootNode, m_LayoutRootNode.children[0], offSet);
        }

        private void Convert2PrimNode(
            IBinaryTreeNode rootPrimNode,
            NodeAutoLayouter.TreeNode rootLayoutNode,
            Vector2 offSet
        )
        {
            var children = rootPrimNode.GetBinaryTreeChildren();
            for (int i = 0; i < rootLayoutNode.children.Count; i++)
            {
                Convert2PrimNode(children[i], rootLayoutNode.children[i], offSet);
                Vector2 calculateResult = rootLayoutNode.children[i].GetPos();
                if (children[i].View is GraphElement graphElement)
                    graphElement.SetPosition(new Rect(calculateResult + offSet, graphElement.contentRect.size));
            }
        }
    }
}
