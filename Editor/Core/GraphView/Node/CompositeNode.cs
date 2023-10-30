using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class CompositeNode : BehaviorTreeNode
    {
        public bool NoValidate { get; private set; }
        public List<Port> ChildPorts = new();
        private readonly List<IBehaviorTreeNode> cache = new();
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = ScriptableObject.CreateInstance<CompositeSearchWindowProvider>();
                provider.Init(this, BehaviorTreeSetting.GetMask(MapTreeView.TreeEditorName));
                SearchWindow.Open(new SearchWindowContext(a.eventInfo.localMousePosition), provider);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Add Child", (a) => AddChild()));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Remove Unnecessary Children", (a) => RemoveUnnecessaryChildren()));
            base.BuildContextualMenu(evt);
        }

        public CompositeNode()
        {
            AddToClassList(nameof(CompositeNode));
            AddChild();
        }

        public void AddChild()
        {
            var child = CreateChildPort();
            ChildPorts.Add(child);
            outputContainer.Add(child);
        }
        protected override void OnBehaviorSet()
        {
            NoValidate = GetBehavior().GetCustomAttribute(typeof(NoValidateAttribute), false) != null;
        }
        public void RemoveUnnecessaryChildren()
        {
            var unnecessary = ChildPorts.Where(p => !p.connected).ToList();
            unnecessary.ForEach(e =>
            {
                ChildPorts.Remove(e);
                outputContainer.Remove(e);
            });
        }

        protected override bool OnValidate(Stack<IBehaviorTreeNode> stack)
        {
            if (ChildPorts.Count <= 0 && !NoValidate) return false;
            foreach (var port in ChildPorts)
            {
                if (!port.connected)
                {
                    if (NoValidate) continue;
                    style.backgroundColor = Color.red;
                    return false;
                }
                stack.Push(PortHelper.FindChildNode(port));
            }
            style.backgroundColor = new StyleColor(StyleKeyword.Null);
            return true;
        }

        protected override void OnCommit(Stack<IBehaviorTreeNode> stack)
        {
            cache.Clear();
            foreach (var port in ChildPorts)
            {
                if (port.connections.Count() == 0) continue;
                var child = PortHelper.FindChildNode(port);
                (NodeBehavior as Composite).AddChild(child.ReplaceBehavior());
                stack.Push(child);
                cache.Add(child);
            }
        }

        protected override void OnClearStyle()
        {
            foreach (var node in cache)
            {
                node.ClearStyle();
            }
        }
        public override IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren()
        {
            return ChildPorts.Where(x => x.connected)
                    .Select(x => PortHelper.FindChildNode(x))
                    .OfType<ILayoutTreeNode>()
                    .Reverse()
                    .ToList();
        }
    }
}