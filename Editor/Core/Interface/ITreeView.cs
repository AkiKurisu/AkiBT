using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public interface ITreeView
    {
        EditorWindow EditorWindow { get; }
        GraphView View { get; }
        IControlGroupBlock GroupBlockController { get; }
        /// <summary>
        /// 结点选择委托
        /// </summary>
        /// <value></value>
        Action<IBehaviorTreeNode> OnNodeSelect { get; }
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
        /// 暴露的共享变量
        /// Exposed SharedVariables
        /// </summary>
        /// <value></value>
        List<SharedVariable> ExposedProperties { get; }
        IBlackBoard BlackBoard { get; }
    }
}
