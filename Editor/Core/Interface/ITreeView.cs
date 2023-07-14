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
        /// 编辑器名称(例如AkiBT,AkiDT)
        /// </summary>
        string TreeEditorName{get;}
        /// <summary>
        /// 共享变量名称修改事件
        /// </summary>
        event System.Action<SharedVariable> OnPropertyNameChange;
        /// <summary>
        /// 编辑器内共享变量
        /// </summary>
        /// <value></value>
        List<SharedVariable> ExposedProperties{get;}
        bool IsRestoring{get;}
        /// <summary>
        /// 添加共享变量到黑板
        /// </summary>
        /// <param name="variable"></param>
        void AddPropertyToBlackBoard(SharedVariable variable);
    }
}
