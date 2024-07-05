using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public interface ITreeView : IVariableSource
    {
        /// <summary>
        /// The editor name
        /// </summary>
        string EditorName { get; }
        IBehaviorTreeContainer Container { get; }
        IBlackBoard BlackBoard { get; }
        EditorWindow EditorWindow { get; }
        GraphView View { get; }
        /// <summary>
        /// Callback on node select
        /// </summary>
        /// <value></value>
        Action<IBehaviorTreeNode> OnNodeSelect { get; }
        /// <summary>
        /// Duplicate a behavior tree node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IBehaviorTreeNode DuplicateNode(IBehaviorTreeNode node);
    }
}
