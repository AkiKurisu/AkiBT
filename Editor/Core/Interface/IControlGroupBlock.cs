using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public interface IControlGroupBlock
    {
        /// <summary>
        /// 将选中结点加入Group并创建Block
        /// </summary>
        /// <param name="node"></param>
        void SelectGroup(IBehaviorTreeNode node);
        /// <summary>
        /// 取消Group
        /// </summary>
        void UnSelectGroup();
        /// <summary>
        /// 创建Group
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="blockData"></param>
        /// <returns></returns>
        GroupBlock CreateBlock(Rect rect, GroupBlockData blockData = null);
    }
}
