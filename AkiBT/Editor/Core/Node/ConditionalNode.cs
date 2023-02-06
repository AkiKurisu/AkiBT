using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ConditionalNode : BehaviorTreeNode
    {
        private Port childPort;

        public Port Child => childPort;

        private BehaviorTreeNode cache;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider =ScriptableObject.CreateInstance< ConditionalSearchWindowProvider>();
                provider.Init(this,BehaviorTreeSetting.GetMask(mapTreeView.treeEditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.mousePosition), provider);
            }));
            base.BuildContextualMenu(evt);
        }

        public ConditionalNode()
        {
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            if (!childPort.connected)
            {
                return true;
            }
            stack.Push(childPort.connections.First().input.node as BehaviorTreeNode);
            return true;
        }

        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            if (!childPort.connected)
            {
                (NodeBehavior as Conditional).Child = null;
                cache = null;
                return;
            }
            var child = childPort.connections.First().input.node as BehaviorTreeNode;
            (NodeBehavior as Conditional).Child = child.ReplaceBehavior();
            stack.Push(child);
            cache = child;
        }

        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
    }
}