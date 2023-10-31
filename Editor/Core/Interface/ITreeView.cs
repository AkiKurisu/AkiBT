using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public interface ITreeView : IVariableSource
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
        IBlackBoard BlackBoard { get; }
    }
}
