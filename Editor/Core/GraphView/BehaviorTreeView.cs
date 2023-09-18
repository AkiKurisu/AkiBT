using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;
using System;
using UnityEditor.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeView : GraphView, ITreeView, IBinaryTreeNode
    {
        public Blackboard _blackboard;
        protected readonly IBehaviorTree behaviorTree;
        public IBehaviorTree BehaviorTree => behaviorTree;
        protected RootNode root;
        private readonly List<SharedVariable> exposedProperties = new();
        public List<SharedVariable> ExposedProperties => exposedProperties;
        private readonly FieldResolverFactory fieldResolverFactory = FieldResolverFactory.Instance;
        protected NodeSearchWindowProvider provider;
        public event Action<SharedVariable> OnPropertyNameChange;
        /// <summary>
        /// 是否可以保存为SO,如果可以会在ToolBar中提供按钮
        /// </summary>
        public virtual bool CanSaveToSO => behaviorTree is BehaviorTree;
        public virtual string TreeEditorName => "AkiBT";
        private readonly NodeResolverFactory nodeResolver = NodeResolverFactory.Instance;
        /// <summary>
        /// 结点选择委托
        /// </summary>
        public Action<IBehaviorTreeNode> onSelectAction;
        private readonly EditorWindow _window;
        private readonly BehaviorNodeConverter converter = new();
        private readonly DragDropManipulator dragDropManipulator;
        public VisualElement View => this;
        public BehaviorTreeView(IBehaviorTree bt, EditorWindow editor)
        {
            _window = editor;
            behaviorTree = bt;
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(BehaviorTreeSetting.GetGraphStyle(TreeEditorName));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());
            var contentDragger = new ContentDragger();
            //鼠标中键移动
            contentDragger.activators.Add(new ManipulatorActivationFilter()
            {
                button = MouseButton.MiddleMouse,
            });
            // 添加选框
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
            this.AddManipulator(contentDragger);
            dragDropManipulator = new DragDropManipulator(this);
            dragDropManipulator.OnDragOverEvent += CopyFromObject;
            this.AddManipulator(dragDropManipulator);
            provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, editor, BehaviorTreeSetting.GetMask(TreeEditorName));
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
            serializeGraphElements += CopyOperation;
            canPasteSerializedData += (data) => true;
            unserializeAndPaste += OnPaste;
        }
        private string CopyOperation(IEnumerable<GraphElement> elements)
        {
            ClearSelection();
            foreach (GraphElement n in elements)
            {
                AddToSelection(n);
            }
            return "Copy Nodes";
        }
        /// <summary>
        /// 粘贴结点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void OnPaste(string a, string b)
        {
            List<ISelectable> copyElements = new CopyPasteGraph(this, selection).GetCopyElements();
            ClearSelection();
            //再次选择
            copyElements.ForEach(node =>
            {
                node.Select(this, true);
            });
        }
        public IBehaviorTreeNode DuplicateNode(IBehaviorTreeNode node)
        {
            var newNode = nodeResolver.Create(node.GetBehavior(), this);
            Rect newRect = node.View.GetPosition();
            newRect.position += new Vector2(50, 50);
            newNode.View.SetPosition(newRect);
            newNode.OnSelectAction = onSelectAction;
            AddElement(newNode.View);
            newNode.CopyFrom(node);
            return newNode;
        }
        public GroupBlock CreateBlock(Rect rect, GroupBlockData blockData = null)
        {
            if (blockData == null) blockData = new GroupBlockData();
            var group = new GroupBlock
            {
                autoUpdateGeometry = true,
                title = blockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }
        internal void NotifyEditSharedVariable(SharedVariable variable)
        {
            OnPropertyNameChange?.Invoke(variable);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var remainTargets = evt.menu.MenuItems().FindAll(e =>
            {
                return e switch
                {
                    BehaviorTreeDropdownMenuAction _ => true,
                    DropdownMenuAction a => a.name == "Create Node" || a.name == "Delete",
                    _ => false,
                };
            });
            //Remove needless default actions .
            evt.menu.MenuItems().Clear();
            remainTargets.ForEach(evt.menu.MenuItems().Add);
        }
        public void AddExposedProperty(SharedVariable variable)
        {
            var localPropertyValue = variable.GetValue();
            if (string.IsNullOrEmpty(variable.Name)) variable.Name = variable.GetType().Name;
            var localPropertyName = variable.Name;
            int index = 1;
            while (ExposedProperties.Any(x => x.Name == localPropertyName))
            {
                localPropertyName = $"{variable.Name}{index}";
                index++;
            }
            variable.Name = localPropertyName;
            ExposedProperties.Add(variable);
            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = variable.GetType().Name };
            field.capabilities &= ~Capabilities.Deletable;
            field.capabilities &= ~Capabilities.Movable;
            container.Add(field);
            FieldInfo info = variable.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var fieldResolver = fieldResolverFactory.Create(info);
            var valueField = fieldResolver.GetEditorField(exposedProperties, variable);
            var placeHolder = new VisualElement();
            placeHolder.Add(valueField);
            if (variable is SharedObject sharedObject)
            {
                placeHolder.Add(GetConstraintField(sharedObject, (ObjectField)valueField));
            }
            var sa = new BlackboardRow(field, placeHolder);
            sa.AddManipulator(new ContextualMenuManipulator((evt) => BuildBlackboardMenu(evt, container)));
            container.Add(sa);
            _blackboard.Add(container);
        }
        private VisualElement GetConstraintField(SharedObject sharedObject, ObjectField objectField)
        {
            const string NonConstraint = "No Constraint";
            var placeHolder = new VisualElement();
            string constraintTypeName;
            try
            {
                objectField.objectType = Type.GetType(sharedObject.ConstraintTypeAQM, true);
                constraintTypeName = "Constraint Type : " + objectField.objectType.Name;
            }
            catch
            {
                objectField.objectType = typeof(UnityEngine.Object);
                constraintTypeName = NonConstraint;
            }
            var typeField = new Label(constraintTypeName);
            placeHolder.Add(typeField);
            var button = new Button()
            {
                text = "Change Constraint Type"
            };
            button.clicked += () =>
             {
                 var provider = ScriptableObject.CreateInstance<ObjectTypeSearchWindow>();
                 provider.Initialize((type) =>
                 {
                     if (type == null)
                     {
                         typeField.text = sharedObject.ConstraintTypeAQM = NonConstraint;
                         objectField.objectType = typeof(UnityEngine.Object);
                     }
                     else
                     {
                         objectField.objectType = type;
                         sharedObject.ConstraintTypeAQM = type.AssemblyQualifiedName;
                         typeField.text = "Constraint Type : " + type.Name;
                     }
                 });
                 SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
             };

            placeHolder.Add(button);
            return placeHolder;
        }
        private void BuildBlackboardMenu(ContextualMenuPopulateEvent evt, VisualElement element)
        {
            evt.menu.MenuItems().Clear();
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Delate Variable", (a) =>
            {
                int index = _blackboard.contentContainer.IndexOf(element);
                ExposedProperties.RemoveAt(index - 1);
                _blackboard.Remove(element);
                return;
            }));
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }
        private void CopyFromObject(UnityEngine.Object data, Vector3 mousePosition)
        {
            if (data is GameObject gameObject)
            {
                if (gameObject.TryGetComponent(out IBehaviorTree tree))
                {
                    _window.ShowNotification(new GUIContent("GameObject Dropped Succeed !"));
                    CopyFromOtherTree(tree, mousePosition);
                    return;
                }
                _window.ShowNotification(new GUIContent("Invalid Drag GameObject !"));
                return;
            }
            if (data is TextAsset asset)
            {
                if (CopyFromJsonFile(asset.text, mousePosition))
                    _window.ShowNotification(new GUIContent("Text Asset Dropped Succeed !"));
                else
                    _window.ShowNotification(new GUIContent("Invalid Drag Text Asset !"));
                return;
            }
            if (data is not IBehaviorTree)
            {
                _window.ShowNotification(new GUIContent("Invalid Drag Data !"));
                return;
            }
            _window.ShowNotification(new GUIContent("Data Dropped Succeed !"));
            CopyFromOtherTree(data as IBehaviorTree, mousePosition);
        }
        internal protected void CopyFromOtherTree(IBehaviorTree otherTree, Vector2 mousePosition)
        {
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition) - new Vector2(400, 300);
            IEnumerable<IBehaviorTreeNode> nodes;
            RootNode rootNode;
            foreach (var variable in otherTree.SharedVariables)
            {
                AddExposedProperty(variable.Clone() as SharedVariable);//Clone新的共享变量
            }
            (rootNode, nodes) = converter.ConvertToNode(otherTree, this, localMousePosition);
            foreach (var node in nodes) node.OnSelectAction = onSelectAction;
            var edge = rootNode.Child.connections.First();
            RemoveElement(edge);
            RemoveElement(rootNode);
            foreach (var nodeBlockData in otherTree.BlockData)
            {
                CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.GUID)).Select(x => x.View));
            }
        }
        internal void Restore()
        {
            IBehaviorTree tree = behaviorTree.ExternalBehaviorTree != null ? behaviorTree.ExternalBehaviorTree : behaviorTree;
            OnRestore(tree);
        }
        protected virtual void OnRestore(IBehaviorTree tree)
        {
            RestoreSharedVariables(tree);
            IEnumerable<IBehaviorTreeNode> nodes;
            (root, nodes) = converter.ConvertToNode(tree, this, Vector2.zero);
            foreach (var node in nodes) node.OnSelectAction = onSelectAction;
            RestoreBlocks(tree, nodes);
        }
        protected void RestoreSharedVariables(IBehaviorTree tree)
        {
            foreach (var variable in tree.SharedVariables)
            {
                AddExposedProperty(variable.Clone() as SharedVariable);//Clone新的共享变量
            }
        }
        protected void RestoreBlocks(IBehaviorTree tree, IEnumerable<IBehaviorTreeNode> nodes)
        {
            foreach (var nodeBlockData in tree.BlockData)
            {
                CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.GUID)).Select(x => x.View));
            }
        }
        void ITreeView.SelectGroup(IBehaviorTreeNode node)
        {
            var block = CreateBlock(new Rect((node as Node).transform.position, new Vector2(100, 100)));
            foreach (var select in selection)
            {
                if (select is not IBehaviorTreeNode || select is RootNode) continue;
                block.AddElement(select as Node);
            }
        }
        void ITreeView.UnSelectGroup()
        {
            foreach (var select in selection)
            {
                if (select is not IBehaviorTreeNode) continue;
                var node = select as Node;
                var block = graphElements.ToList().OfType<GroupBlock>().FirstOrDefault(x => x.ContainsElement(node));
                block?.RemoveElement(node);
            }
        }

        internal bool Save(bool autoSave = false)
        {
            if (Application.isPlaying) return false;
            if (Validate())
            {
                Commit(behaviorTree);
                if (autoSave) Debug.Log($"<color=#3aff48>{TreeEditorName}</color>[{behaviorTree._Object.name}] auto save succeed ! {System.DateTime.Now.ToString()}");
                AssetDatabase.SaveAssets();
                return true;
            }
            if (autoSave) Debug.Log($"<color=#ff2f2f>{TreeEditorName}</color>[{behaviorTree._Object.name}] auto save failed ! {System.DateTime.Now.ToString()}");
            return false;
        }

        protected virtual bool Validate()
        {
            //validate nodes by DFS.
            var stack = new Stack<IBehaviorTreeNode>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!node.Validate(stack))
                {
                    return false;
                }
            }
            return true;
        }
        internal void Commit(IBehaviorTree behaviorTree)
        {
            OnCommit(behaviorTree);
        }
        protected virtual void OnCommit(IBehaviorTree behaviorTree)
        {
            var stack = new Stack<IBehaviorTreeNode>();
            stack.Push(root);
            // save new components
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                node.Commit(stack);
            }
            root.PostCommit(behaviorTree);
            behaviorTree.SharedVariables.Clear();
            foreach (var sharedVariable in ExposedProperties)
            {
                behaviorTree.SharedVariables.Add(sharedVariable);
            }
            List<GroupBlock> NodeBlocks = graphElements.ToList().OfType<GroupBlock>().ToList();
            behaviorTree.BlockData.Clear();
            foreach (var block in NodeBlocks)
            {
                block.Commit(behaviorTree.BlockData);
            }
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(behaviorTree._Object);
        }
        internal static Edge ConnectPorts(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            return tempEdge;
        }
        public string SerializeTreeToJson()
        {
            return BehaviorTreeSerializeUtility.SerializeTree(behaviorTree, false, true);
        }
        internal bool CopyFromJsonFile(string serializedData, Vector3 mousePosition)
        {
            var temp = ScriptableObject.CreateInstance<BehaviorTreeSO>();
            try
            {
                temp.Deserialize(serializedData);
                CopyFromOtherTree(temp, mousePosition);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IReadOnlyList<IBinaryTreeNode> GetBinaryTreeChildren()
        {
            return new List<IBinaryTreeNode>() { root };
        }
    }
}