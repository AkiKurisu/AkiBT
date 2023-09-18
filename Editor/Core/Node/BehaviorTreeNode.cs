using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public interface IBehaviorTreeNode
    {
        Node View { get; }
        string GUID { get; }
        Port Parent { get; }
        Action<IBehaviorTreeNode> OnSelectAction { get; set; }
        void Restore(NodeBehavior behavior);
        void Commit(Stack<IBehaviorTreeNode> stack);
        bool Validate(Stack<IBehaviorTreeNode> stack);
        Type GetBehavior();
        void SetBehavior(Type nodeBehavior, ITreeView ownerTreeView = null);
        void CopyFrom(IBehaviorTreeNode copyNode);
        NodeBehavior ReplaceBehavior();
        void ClearStyle();
        IFieldResolver GetFieldResolver(string fieldName);
    }
    public abstract class BehaviorTreeNode : Node, IBinaryTreeNode, IBehaviorTreeNode
    {
        public Node View => this;
        public string GUID => guid;
        private string guid;
        protected NodeBehavior NodeBehavior { set; get; }

        private Type dirtyNodeBehaviorType;
        public Port Parent { private set; get; }

        private readonly VisualElement container;

        private readonly TextField description;
        public string Description => description.value;
        private readonly FieldResolverFactory fieldResolverFactory;
        public readonly List<IFieldResolver> resolvers = new();
        protected readonly List<FieldInfo> fieldInfos = new();
        public Action<IBehaviorTreeNode> OnSelectAction { get; set; }
        protected ITreeView mapTreeView;
        protected bool noValidate;
        VisualElement IBinaryTreeNode.View => this;
        public override void OnSelected()
        {
            base.OnSelected();
            OnSelectAction?.Invoke(this);
        }

        protected BehaviorTreeNode()
        {
            fieldResolverFactory = FieldResolverFactory.Instance;
            container = new VisualElement();
            description = new TextField();
            guid = Guid.NewGuid().ToString();
            Initialize();
        }
        public IFieldResolver GetFieldResolver(string fieldName)
        {
            int index = fieldInfos.FindIndex(x => x.Name == fieldName);
            if (index != -1) return resolvers[index];
            else return null;
        }

        private void Initialize()
        {
            AddDescription();
            mainContainer.Add(container);
            AddParent();
        }

        protected virtual void AddDescription()
        {
            description.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            description.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            mainContainer.Add(description);
        }
        public void Restore(NodeBehavior behavior)
        {
            NodeBehavior = behavior;
            resolvers.ForEach(e => e.Restore(NodeBehavior));
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            description.value = NodeBehavior.description;
            guid = string.IsNullOrEmpty(behavior.GUID) ? Guid.NewGuid().ToString() : behavior.GUID;
            OnRestore();
        }
        public void CopyFrom(IBehaviorTreeNode copyNode)
        {
            var node = copyNode as BehaviorTreeNode;
            for (int i = 0; i < node.resolvers.Count; i++)
            {
                resolvers[i].Copy(node.resolvers[i]);
            }
            description.value = node.Description;
            NodeBehavior = Activator.CreateInstance(copyNode.GetBehavior()) as NodeBehavior;
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            guid = Guid.NewGuid().ToString();
        }

        protected virtual void OnRestore()
        {

        }

        public NodeBehavior ReplaceBehavior()
        {
            NodeBehavior = Activator.CreateInstance(GetBehavior()) as NodeBehavior;
            return NodeBehavior;
        }

        protected virtual void AddParent()
        {
            Parent = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            Parent.portName = "Parent";
            inputContainer.Add(Parent);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
            RegisterCallback<AttachToPanelEvent>(OnAttach);
        }
        private void OnAttach(AttachToPanelEvent evt)
        {
            if (GetFirstAncestorOfType<CompositeStack>() == null) return;
            if (Parent.connected)
            {
                var edge = Parent.connections.First();
                edge.input?.Disconnect(edge);
                edge.RemoveFromHierarchy();
            }
            Parent.SetEnabled(false);
        }
        private void OnDetach(DetachFromPanelEvent evt)
        {
            Parent.SetEnabled(true);
        }
        protected Port CreateChildPort()
        {
            var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Port));
            port.portName = "Child";
            return port;
        }

        public Type GetBehavior()
        {
            return dirtyNodeBehaviorType;
        }

        public void Commit(Stack<IBehaviorTreeNode> stack)
        {
            OnCommit(stack);
            resolvers.ForEach(r => r.Commit(NodeBehavior));
            NodeBehavior.description = description.value;
            NodeBehavior.graphPosition = GetPosition();
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            NodeBehavior.GUID = GUID;
        }
        protected abstract void OnCommit(Stack<IBehaviorTreeNode> stack);

        public bool Validate(Stack<IBehaviorTreeNode> stack)
        {
            var valid = GetBehavior() != null && OnValidate(stack);
            if (valid)
            {
                style.backgroundColor = new StyleColor(StyleKeyword.Null);
            }
            else
            {
                style.backgroundColor = Color.red;
            }
            return valid;
        }

        protected abstract bool OnValidate(Stack<IBehaviorTreeNode> stack);
        /// <summary>
        ///  核心:设置结点行为类型
        /// </summary>
        /// <param name="nodeBehavior"></param>
        public void SetBehavior(Type nodeBehavior, ITreeView ownerTreeView = null)
        {
            if (ownerTreeView != null) this.mapTreeView = ownerTreeView;
            if (dirtyNodeBehaviorType != null)
            {
                dirtyNodeBehaviorType = null;
                container.Clear();
                resolvers.Clear();
                fieldInfos.Clear();
            }
            dirtyNodeBehaviorType = nodeBehavior;

            nodeBehavior
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null)
                .Concat(GetAllFields(nodeBehavior))
                .Where(field => field.IsInitOnly == false)
                .ToList().ForEach((p) =>
                {
                    var fieldResolver = fieldResolverFactory.Create(p);
                    var defaultValue = Activator.CreateInstance(nodeBehavior) as NodeBehavior;
                    fieldResolver.Restore(defaultValue);
                    container.Add(fieldResolver.GetEditorField(mapTreeView));
                    resolvers.Add(fieldResolver);
                    fieldInfos.Add(p);
                });
            var label = nodeBehavior.GetCustomAttribute(typeof(AkiLabelAttribute), false) as AkiLabelAttribute;
            title = label?.Title ?? nodeBehavior.Name;
            noValidate = nodeBehavior.GetCustomAttribute(typeof(NoValidateAttribute), false) != null;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null).Concat(GetAllFields(t.BaseType));//Concat合并列表
        }

        private void MarkAsExecuted(Status status)
        {
            switch (status)
            {
                case Status.Failure:
                    {
                        style.backgroundColor = Color.red;
                        break;
                    }
                case Status.Running:
                    {
                        style.backgroundColor = Color.yellow;
                        break;
                    }
                case Status.Success:
                    {
                        style.backgroundColor = Color.green;
                        break;
                    }
            }
        }

        public void ClearStyle()
        {
            style.backgroundColor = new StyleColor(StyleKeyword.Null);
            OnClearStyle();
        }

        protected abstract void OnClearStyle();
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Duplicate", (a) =>
            {
                mapTreeView.DuplicateNode(this);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Select Group", (a) =>
            {
                mapTreeView.SelectGroup(this);
                return;
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("UnSelect Group", (a) =>
            {
                mapTreeView.UnSelectGroup();
                return;
            }));
        }

        public abstract IReadOnlyList<IBinaryTreeNode> GetBinaryTreeChildren();
    }
}