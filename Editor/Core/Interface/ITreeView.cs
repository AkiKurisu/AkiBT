using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public interface ITreeView
    {
        GraphView View { get; }
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
        /// 复制结点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IBehaviorTreeNode DuplicateNode(IBehaviorTreeNode node);
        /// <summary>
        /// 编辑器名称
        /// </summary>
        string TreeEditorName { get; }
        /// <summary>
        /// 共享变量名称修改事件
        /// </summary>
        event Action<SharedVariable> OnPropertyNameChange;
        /// <summary>
        /// 暴露的共享变量
        /// Exposed SharedVariables
        /// </summary>
        /// <value></value>
        List<SharedVariable> ExposedProperties { get; }
        IBlackBoard BlackBoard { get; }
    }
}
