using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeView : GraphView, ITreeView, ILayoutTreeNode
    {
        public IBlackBoard BlackBoard { get; internal set; }
        protected readonly IBehaviorTree behaviorTree;
        public IBehaviorTree BehaviorTree => behaviorTree;
        protected RootNode root;
        private readonly List<SharedVariable> exposedProperties = new();
        public List<SharedVariable> ExposedProperties => exposedProperties;
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
        private readonly BehaviorNodeConvertor converter = new();
        VisualElement ILayoutTreeNode.View => this;
        public GraphView View => this;
        public BehaviorTreeView(IBehaviorTree bt, EditorWindow editor)
        {
            _window = editor;
            behaviorTree = bt;
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(BehaviorTreeSetting.GetGraphStyle(TreeEditorName));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());
            AddManipulators();
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
        protected virtual void AddManipulators()
        {
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());
            var contentDragger = new ContentDragger();
            contentDragger.activators.Add(new ManipulatorActivationFilter()
            {
                button = MouseButton.MiddleMouse,
            });
            this.AddManipulator(contentDragger);
            this.AddManipulator(new DragDropManipulator(CopyFromObject));
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
        private void OnPaste(string a, string b)
        {
            List<ISelectable> copyElements = new CopyPasteGraph(this, selection).GetCopyElements();
            ClearSelection();
            copyElements.ForEach(node =>
            {
                node.Select(this, true);
            });
        }
        public IBehaviorTreeNode DuplicateNode(IBehaviorTreeNode node)
        {
            var newNode = nodeResolver.Create(node.GetBehavior(), this);
            Rect newRect = node.GetWorldPosition();
            newRect.position += new Vector2(50, 50);
            newNode.View.SetPosition(newRect);
            newNode.OnSelectAction = onSelectAction;
            AddElement(newNode.View);
            newNode.CopyFrom(node);
            return newNode;
        }
        public GroupBlock CreateBlock(Rect rect, GroupBlockData blockData = null)
        {
            blockData ??= new GroupBlockData();
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
        public void CopyFromObject(UnityEngine.Object data, Vector2 mousePosition)
        {
            if (data is GameObject gameObject)
            {
                if (gameObject.TryGetComponent(out IBehaviorTree tree))
                {
                    _window.ShowNotification(new GUIContent("GameObject Dropped Succeed !"));
                    CopyFromTree(tree, mousePosition);
                    return;
                }
                _window.ShowNotification(new GUIContent("Invalid Drag GameObject !"));
                return;
            }
            if (data is TextAsset asset)
            {
                if (CopyFromJson(asset.text, mousePosition))
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
            CopyFromTree(data as IBehaviorTree, mousePosition);
        }
        /// <summary>
        /// Copy graph view nodes from other tree
        /// </summary>
        /// <param name="otherTree"></param>
        /// <param name="mousePosition"></param>
        public void CopyFromTree(IBehaviorTree otherTree, Vector2 mousePosition)
        {
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition) - new Vector2(400, 300);
            IEnumerable<IBehaviorTreeNode> nodes;
            RootNode rootNode;
            RestoreSharedVariables(otherTree);
            (rootNode, nodes) = converter.ConvertToNode(otherTree, this, localMousePosition);
            foreach (var node in nodes) node.OnSelectAction = onSelectAction;
            var edge = rootNode.Child.connections.First();
            RemoveElement(edge);
            RemoveElement(rootNode);
            RestoreBlocks(otherTree, nodes);
        }
        /// <summary>
        /// Serialize current editing behavior tree to json format
        /// </summary>
        /// <returns></returns>
        public string SerializeTreeToJson()
        {
            return BehaviorTreeSerializeUtility.SerializeTree(behaviorTree, false, true);
        }
        /// <summary>
        /// Copy BehaviorTree from json file
        /// </summary>
        /// <param name="serializedData">json data</param>
        /// <param name="mousePosition">drag or init position</param>
        /// <returns></returns>
        public bool CopyFromJson(string serializedData, Vector3 mousePosition)
        {
            var temp = ScriptableObject.CreateInstance<BehaviorTreeSO>();
            try
            {
                temp.Deserialize(serializedData);
                CopyFromTree(temp, mousePosition);
                return true;
            }
            catch
            {
                return false;
            }
        }
        internal void Restore()
        {
            IBehaviorTree tree = behaviorTree.ExternalBehaviorTree != null ? behaviorTree.ExternalBehaviorTree : behaviorTree;
            OnRestore(tree);
        }
        /// <summary>
        /// Restore behavior tree to the graph view nodes
        /// </summary>
        /// <param name="tree"></param>
        protected virtual void OnRestore(IBehaviorTree tree)
        {
            RestoreSharedVariables(tree);
            IEnumerable<IBehaviorTreeNode> nodes;
            (root, nodes) = converter.ConvertToNode(tree, this, Vector2.zero);
            foreach (var node in nodes) node.OnSelectAction = onSelectAction;
            RestoreBlocks(tree, nodes);
        }
        private void RestoreSharedVariables(IBehaviorTree tree)
        {
            foreach (var variable in tree.SharedVariables)
            {
                BlackBoard.AddExposedProperty(variable.Clone() as SharedVariable);
            }
        }
        private void RestoreBlocks(IBehaviorTree tree, IEnumerable<IBehaviorTreeNode> nodes)
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
                var block = graphElements.OfType<GroupBlock>().FirstOrDefault(x => x.ContainsElement(node));
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
            List<GroupBlock> NodeBlocks = graphElements.OfType<GroupBlock>().ToList();
            behaviorTree.BlockData.Clear();
            foreach (var block in NodeBlocks)
            {
                block.Commit(behaviorTree.BlockData);
            }
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(behaviorTree._Object);
        }
        public IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren()
        {
            return new List<ILayoutTreeNode>() { root };
        }
    }
}