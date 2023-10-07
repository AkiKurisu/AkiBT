using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    internal class ParentBridge : Node
    {
        public Port Parent { get; }
        public ParentBridge() : base()
        {
            AddToClassList("BridgeNode");
            capabilities &= ~Capabilities.Copiable;
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Movable;
            title = "Parent";
            Parent = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            Parent.portName = string.Empty;
            inputContainer.Add(Parent);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        private void OnDetach(DetachFromPanelEvent evt)
        {
            //Fix edge remain in the graph though Stack is removed
            if (Parent.connected)
            {
                var edge = Parent.connections.First();
                edge.input?.Disconnect(edge);
                edge.RemoveFromHierarchy();
            }
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            evt.StopPropagation();
        }
    }
    public abstract class CompositeStack : StackNode, IBehaviorTreeNode, ILayoutTreeNode
    {
        public CompositeStack() : base()
        {
            capabilities |= Capabilities.Groupable;
            fieldResolverFactory = FieldResolverFactory.Instance;
            fieldContainer = new VisualElement();
            GUID = Guid.NewGuid().ToString();
            Initialize();
            headerContainer.style.flexDirection = FlexDirection.Column;
            headerContainer.style.justifyContent = Justify.Center;
            titleLabel = new Label();
            headerContainer.Add(titleLabel);
            description = new TextField
            {
                multiline = true
            };
            description.style.whiteSpace = WhiteSpace.Normal;
            AddDescription();
            AddToClassList(nameof(CompositeStack));
            //Replace dark color of the place holder
            this.Q<VisualElement>(classes: "stack-node-placeholder")
                .Children()
                .First().style.color = new Color(1, 1, 1, 0.6f);
            var bridge = new ParentBridge();
            Parent = bridge.Parent;
            Parent.portColor = PortColor;
            AddElement(bridge);
        }
        public Node View => this;
        public virtual Color PortColor => new(97 / 255f, 95 / 255f, 212 / 255f, 0.91f);
        private readonly TextField description;
        public string GUID { get; private set; }
        protected NodeBehavior NodeBehavior { set; get; }
        private readonly Label titleLabel;
        private Type dirtyNodeBehaviorType;
        public Port Parent { get; }
        private readonly VisualElement fieldContainer;
        private readonly FieldResolverFactory fieldResolverFactory;
        private readonly List<IFieldResolver> resolvers = new();
        private readonly List<FieldInfo> fieldInfos = new();
        public Action<IBehaviorTreeNode> OnSelectAction { get; set; }

        VisualElement ILayoutTreeNode.View => this;

        public ITreeView MapTreeView { get; private set; }
        public IFieldResolver GetFieldResolver(string fieldName)
        {
            int index = fieldInfos.FindIndex(x => x.Name == fieldName);
            if (index != -1) return resolvers[index];
            else return null;
        }
        private void Initialize()
        {
            mainContainer.Add(fieldContainer);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            OnSelectAction?.Invoke(this);
        }
        private void AddDescription()
        {
            description.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            description.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
            headerContainer.Add(description);
        }
        public void Restore(NodeBehavior behavior)
        {
            NodeBehavior = behavior;
            resolvers.ForEach(e => e.Restore(NodeBehavior));
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            description.value = NodeBehavior.description;
            GUID = string.IsNullOrEmpty(behavior.GUID) ? Guid.NewGuid().ToString() : behavior.GUID;
        }
        public void CopyFrom(IBehaviorTreeNode copyNode)
        {
            var node = copyNode as CompositeStack;
            for (int i = 0; i < node.resolvers.Count; i++)
            {
                resolvers[i].Copy(node.resolvers[i]);
            }
            copyMapCache.Clear();
            node.contentContainer.Query<BehaviorTreeNode>()
            .ToList()
            .ForEach(
                x =>
                {
                    var newNode = MapTreeView.DuplicateNode(x);
                    if (x is IChildPortable portable)
                        copyMapCache[x.GetHashCode()] = portable;
                    AddElement(newNode.View);
                }
            );
            description.value = node.description.value;
            NodeBehavior = (NodeBehavior)Activator.CreateInstance(copyNode.GetBehavior());
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            GUID = Guid.NewGuid().ToString();
        }
        private readonly Dictionary<int, IChildPortable> copyMapCache = new();
        internal IReadOnlyDictionary<int, IChildPortable> GetCopyMap()
        {
            return copyMapCache;
        }
        public NodeBehavior ReplaceBehavior()
        {
            NodeBehavior = (NodeBehavior)Activator.CreateInstance(GetBehavior());
            return NodeBehavior;
        }
        public Type GetBehavior()
        {
            return dirtyNodeBehaviorType;
        }

        public void Commit(Stack<IBehaviorTreeNode> stack)
        {
            OnCommit(stack);
            var nodes = contentContainer.Query<BehaviorTreeNode>().ToList();
            nodes.ForEach(x =>
            {
                (NodeBehavior as Composite).AddChild(x.ReplaceBehavior());
                stack.Push(x);
            });
            //Manually commit bridge node
            //Do not duplicate commit dialogue piece
            resolvers.ForEach(r => r.Commit(NodeBehavior));
            NodeBehavior.description = description.value;
            NodeBehavior.graphPosition = GetPosition();
            NodeBehavior.NotifyEditor = MarkAsExecuted;
            NodeBehavior.GUID = GUID;
        }
        protected virtual void OnCommit(Stack<IBehaviorTreeNode> stack) { }
        public bool Validate(Stack<IBehaviorTreeNode> stack)
        {
            contentContainer.Query<BehaviorTreeNode>()
                            .ForEach(x => stack.Push(x));
            var valid = GetBehavior() != null;
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
        public void SetBehavior(Type nodeBehavior, ITreeView ownerTreeView = null)
        {
            if (ownerTreeView != null) MapTreeView = ownerTreeView;
            if (dirtyNodeBehaviorType != null)
            {
                dirtyNodeBehaviorType = null;
                fieldContainer.Clear();
                resolvers.Clear();
                fieldInfos.Clear();
            }
            dirtyNodeBehaviorType = nodeBehavior;

            var defaultValue = (NodeBehavior)Activator.CreateInstance(nodeBehavior);
            nodeBehavior
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null)
                .Concat(GetAllFields(nodeBehavior))
                .Where(field => field.IsInitOnly == false)
                .ToList().ForEach((p) =>
                {
                    var fieldResolver = fieldResolverFactory.Create(p);
                    fieldResolver.Restore(defaultValue);

                    fieldContainer.Add(fieldResolver.GetEditorField(MapTreeView));

                    resolvers.Add(fieldResolver);
                    fieldInfos.Add(p);
                });
            var label = nodeBehavior.GetCustomAttribute(typeof(AkiLabelAttribute), false) as AkiLabelAttribute;
            titleLabel.text = label?.Title ?? nodeBehavior.Name;
        }
        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null).Concat(GetAllFields(t.BaseType));
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
            contentContainer.Query<BehaviorTreeNode>()
                            .ForEach(x => x.ClearStyle());
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Duplicate", (a) =>
            {
                MapTreeView.DuplicateNode(this);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Select Group", (a) =>
            {
                MapTreeView.GroupBlockController.SelectGroup(this);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("UnSelect Group", (a) =>
            {
                MapTreeView.GroupBlockController.UnSelectGroup();
            }));
        }

        public IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren()
        {
            var list = new List<ILayoutTreeNode>();
            contentContainer
            .Query<BehaviorTreeNode>()
            .ToList()
            .OfType<ILayoutTreeNode>()
            .Reverse()
            .ToList()
            .ForEach(x => list.AddRange(x.GetLayoutTreeChildren()));
            return list;
        }
        public Rect GetWorldPosition() => GetPosition();
    }
}
