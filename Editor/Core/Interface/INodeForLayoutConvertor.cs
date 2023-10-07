using System.Collections.Generic;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public interface ILayoutTreeNode
    {
        VisualElement View { get; }
        IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren();
    }
    /// <summary>
    /// Modfied from https://gitee.com/NKG_admin/NKGMobaBasedOnET/tree/master/Unity/Assets/Model/NKGMOBA/Helper/NodeGraph/Core
    /// </summary>
    public interface INodeForLayoutConvertor
    {
        /// <summary>
        /// 节点间的距离
        /// </summary>
        float SiblingDistance { get; }
        ILayoutTreeNode PrimRootNode { get; }
        NodeAutoLayoutHelper.TreeNode LayoutRootNode { get; }

        INodeForLayoutConvertor Init(ILayoutTreeNode primRootNode);
        NodeAutoLayoutHelper.TreeNode PrimNode2LayoutNode();
        void LayoutNode2PrimNode();
    }
}
