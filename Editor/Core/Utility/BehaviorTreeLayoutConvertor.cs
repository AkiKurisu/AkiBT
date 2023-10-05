using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeLayoutConvertor : INodeForLayoutConvertor
    {
        public float SiblingDistance { get; private set; }
        private NodeAutoLayoutHelper.TreeNode m_LayoutRootNode;
        public NodeAutoLayoutHelper.TreeNode LayoutRootNode => m_LayoutRootNode;
        public ILayoutTreeNode PrimRootNode => m_PrimRootNode;
        private ILayoutTreeNode m_PrimRootNode;
        private const NodeAutoLayoutHelper.CalculateMode CalculateMode = NodeAutoLayoutHelper.CalculateMode.Horizontal | NodeAutoLayoutHelper.CalculateMode.Positive;
        public INodeForLayoutConvertor Init(ILayoutTreeNode primRootNode)
        {
            m_PrimRootNode = primRootNode;
            SiblingDistance = BehaviorTreeSetting.GetOrCreateSettings().AutoLayoutSiblingDistance;
            return this;
        }
        public NodeAutoLayoutHelper.TreeNode PrimNode2LayoutNode()
        {
            if (m_PrimRootNode.View.layout.width == float.NaN)
            {
                return null;
            }

            m_LayoutRootNode =
                new NodeAutoLayoutHelper.TreeNode(m_PrimRootNode.View.layout.height + SiblingDistance,
                    m_PrimRootNode.View.layout.width,
                    m_PrimRootNode.View.layout.y,
                    CalculateMode);

            Convert2LayoutNode(m_PrimRootNode,
                m_LayoutRootNode, m_PrimRootNode.View.layout.y + m_PrimRootNode.View.layout.width,
                CalculateMode);
            return m_LayoutRootNode;
        }

        private void Convert2LayoutNode(ILayoutTreeNode rootPrimNode,
            NodeAutoLayoutHelper.TreeNode rootLayoutNode, float lastHeightPoint,
            NodeAutoLayoutHelper.CalculateMode calculateMode)
        {
            foreach (var childNode in rootPrimNode.GetLayoutTreeChildren())
            {
                NodeAutoLayoutHelper.TreeNode childLayoutNode =
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
            var rootNode = m_PrimRootNode.GetLayoutTreeChildren()[0];
            var offSet = (rootNode.View as GraphElement).contentRect.position - m_LayoutRootNode.children[0].GetPos() + new Vector2(400, 300);
            Convert2PrimNode(rootNode, m_LayoutRootNode.children[0], offSet);
        }

        private void Convert2PrimNode(
            ILayoutTreeNode rootPrimNode,
            NodeAutoLayoutHelper.TreeNode rootLayoutNode,
            Vector2 offSet
        )
        {
            var children = rootPrimNode.GetLayoutTreeChildren();
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
