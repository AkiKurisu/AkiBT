using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ConditionalNode : BehaviorTreeNode, IChildPortable
    {
        private readonly Port childPort;

        public Port Child => childPort;

        private IBehaviorTreeNode cache;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = ScriptableObject.CreateInstance<ConditionalSearchWindowProvider>();
                provider.Init(this, BehaviorTreeSetting.GetOrCreateSettings().GetMask(MapTreeView.EditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.localMousePosition), provider);
            }));
            base.BuildContextualMenu(evt);
        }

        public ConditionalNode()
        {
            AddToClassList(nameof(ConditionalNode));
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }

        protected override bool OnValidate(Stack<IBehaviorTreeNode> stack)
        {
            if (!childPort.connected)
            {
                return true;
            }
            stack.Push(PortHelper.FindChildNode(childPort));
            return true;
        }

        protected override void OnCommit(Stack<IBehaviorTreeNode> stack)
        {
            if (!childPort.connected)
            {
                (NodeBehavior as Conditional).Child = null;
                cache = null;
                return;
            }
            var child = PortHelper.FindChildNode(childPort);
            (NodeBehavior as Conditional).Child = child.ReplaceBehavior();
            stack.Push(child);
            cache = child;
        }

        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
        public override IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren()
        {
            if (!childPort.connected)
            {
                return new List<ILayoutTreeNode>();
            }
            return new List<ILayoutTreeNode>() { childPort.connections.First().input.node as ILayoutTreeNode };
        }
    }
}