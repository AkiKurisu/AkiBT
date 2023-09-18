using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public sealed class RootNode : BehaviorTreeNode
    {
        public readonly Port Child;

        private IBehaviorTreeNode cache;

        public RootNode()
        {
            SetBehavior(typeof(Root));
            title = "Root";
            Child = CreateChildPort();
            outputContainer.Add(Child);
            capabilities &= ~Capabilities.Copiable;
            capabilities &= ~Capabilities.Deletable;//不可删除
            capabilities &= ~Capabilities.Movable;//不可删除
            RefreshExpandedState();
            RefreshPorts();//更新链接
        }

        protected override void AddParent()
        {
        }

        protected override void AddDescription()
        {
        }

        protected override void OnRestore()
        {
            (NodeBehavior as Root).UpdateEditor = ClearStyle;
        }

        protected override bool OnValidate(Stack<IBehaviorTreeNode> stack)
        {
            if (!Child.connected)
            {
                return true;
            }
            stack.Push(PortHelper.FindChildNode(Child));
            return true;
        }
        protected override void OnCommit(Stack<IBehaviorTreeNode> stack)
        {

            var newRoot = new Root();
            IBehaviorTreeNode child = null;
            if (Child.connected)
            {
                child = PortHelper.FindChildNode(Child);
                newRoot.Child = child.ReplaceBehavior();
                stack.Push(child);

            }
            newRoot.UpdateEditor = ClearStyle;
            NodeBehavior = newRoot;
            cache = child;
        }

        public void PostCommit(IBehaviorTree tree)
        {
            tree.Root = NodeBehavior as Root;
        }
        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
        }
        public override IReadOnlyList<IBinaryTreeNode> GetBinaryTreeChildren()
        {
            if (!Child.connected)
            {
                return new List<IBinaryTreeNode>();
            }
            return new List<IBinaryTreeNode>() { PortHelper.FindChildNode(Child) as IBinaryTreeNode };
        }
    }
}