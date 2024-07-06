using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public sealed class SubtreeNode : BehaviorTreeNode
    {
        private BehaviorTreeAsset subtree;
        public SubtreeNode()
        {
            AddToClassList(nameof(SubtreeNode));
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override bool OnValidate(Stack<IBehaviorTreeNode> stack) => true;
        protected override void OnRestore()
        {
            var node = NodeBehavior as Subtree;
            if (!node.subtree)
            {
                // happen when subtree was deleted
                title = "<color=#FFE000><b>Subtree Missing!</b></color>";
                return;
            }
            subtree = node.subtree;
            title = $"Subtree {subtree.name}";
            foreach (var variable in subtree.GetBehaviorTree().SharedVariables)
            {
                // not add if exist
                if (variable.IsExposed && !MapTreeView.SharedVariables.Any(x => x.Name == variable.Name))
                    MapTreeView.BlackBoard.AddSharedVariable(variable);
            }
        }
        protected override void OnCommit(Stack<IBehaviorTreeNode> stack)
        {
            (NodeBehavior as Subtree).subtree = subtree;
        }

        protected override void OnClearStyle()
        {
        }
        private void OnMouseDown(MouseDownEvent eventData)
        {
            // double click to open subtree editor
            if (eventData.clickCount < 2) return;

            var subtree = NodeBehavior as Subtree;
            if (!subtree.subtree) return;
            GraphEditorWindow.Show(subtree);
        }
    }
}