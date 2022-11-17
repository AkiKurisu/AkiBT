using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace Kurisu.AkiBT.Editor
{
    public sealed class RootNode : BehaviorTreeNode
    {
        public readonly Port Child;

        private BehaviorTreeNode cache;

        public RootNode() 
        {
            SetBehavior(typeof(Root));
            title = "Root";
            Child = CreateChildPort();
            outputContainer.Add(Child);
            capabilities &=~Capabilities.Deletable;//不可删除
            capabilities &=~Capabilities.Movable;//不可删除
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

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            if (!Child.connected)
            {
                return false;
            }
            stack.Push(Child.connections.First().input.node as BehaviorTreeNode);
            return true;
        }
        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            var child = Child.connections.First().input.node as BehaviorTreeNode;
            var newRoot = new Root();
            newRoot.Child = child.ReplaceBehavior();
            newRoot.UpdateEditor = ClearStyle;
            NodeBehavior = newRoot;
            stack.Push(child);
            cache = child;
        }

        public void PostCommit(BehaviorTree tree)
        {
            tree.Root = (NodeBehavior as Root); 
        }
         public void PostCommit(BehaviorTreeSO treeSO)
        {
            treeSO.SetRoot(NodeBehavior as Root); 
        }
        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
    }
}