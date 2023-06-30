using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    public interface ITreeView
    {
        /// <summary>
        /// 将选中结点加入Group并创建Block
        /// </summary>
        /// <param name="node"></param>
        void SelectGroup(BehaviorTreeNode node);
        /// <summary>
        /// 取消Group
        /// </summary>
        void UnSelectGroup();
        /// <summary>
        /// 复制结点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        BehaviorTreeNode DuplicateNode(BehaviorTreeNode node);
        /// <summary>
        /// 编辑器名称
        /// </summary>
        string TreeEditorName{get;}
        /// <summary>
        /// 共享变量名称修改事件(手动触发)
        /// </summary>
        event System.Action<SharedVariable> OnPropertyNameChangeEvent;
        /// <summary>
        /// 共享变量名称编辑事件(自动触发)
        /// </summary>
        event System.Action<SharedVariable> OnPropertyNameEditingEvent;
        /// <summary>
        /// 编辑器内共享变量
        /// </summary>
        /// <value></value>
        List<SharedVariable> ExposedProperties{get;}
        /// <summary>
        /// 是否在Restore中
        /// </summary>
        /// <value></value>
        bool IsRestored{get;}
        /// <summary>
        /// 添加共享变量到黑板
        /// </summary>
        /// <param name="variable"></param>
        void AddPropertyToBlackBoard(SharedVariable variable);
    }
}
