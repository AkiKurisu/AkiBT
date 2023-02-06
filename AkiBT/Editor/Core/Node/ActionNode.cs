using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ActionNode : BehaviorTreeNode
    {
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider =ScriptableObject.CreateInstance<ActionSearchWindowProvider>();
                provider.Init(this,BehaviorTreeSetting.GetMask(mapTreeView.treeEditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.mousePosition), provider);
            }));
            base.BuildContextualMenu(evt);
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack) => true;

        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
        }

        protected override void OnClearStyle()
        {
        }
    }
}