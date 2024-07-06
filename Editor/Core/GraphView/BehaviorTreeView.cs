using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using UnityEngine.Assertions;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeView : GraphView, ITreeView, ILayoutTreeNode
    {
        public IBlackBoard BlackBoard { get; protected set; }
        public BehaviorTree Instance { get; protected set; }
        public IBehaviorTreeContainer Container { get; protected set; }
        protected RootNode root;
        public List<SharedVariable> SharedVariables { get; } = new();
        public virtual string EditorName => "AkiBT";
        private readonly NodeResolverFactory nodeResolver = NodeResolverFactory.Instance;
        public Action<IBehaviorTreeNode> OnNodeSelect { get; set; }
        public EditorWindow EditorWindow { get; internal set; }
        private readonly BehaviorNodeConvertor converter = new();
        VisualElement ILayoutTreeNode.View => this;
        public GraphView View => this;
        public BehaviorTreeView(IBehaviorTreeContainer container, EditorWindow editor)
        {
            EditorWindow = editor;
            Container = container;
            Instance = container.GetBehaviorTree();
            Assert.IsNotNull(Instance.root);
            LocalConstruct();
        }
        /// <summary>
        /// Build tree view elements
        /// </summary>
        private void LocalConstruct()
        {
            SetStyle();
            AddManipulators();
            AddNodeProvider();
            RegisterSerializationCallBack();
            AddBlackBoard();
        }
        private void SetStyle()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(BehaviorTreeSetting.GetGraphStyle(EditorName));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());
        }
        private void AddNodeProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, BehaviorTreeSetting.GetMask(EditorName));
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
        }
        private void RegisterSerializationCallBack()
        {
            serializeGraphElements += OnSerialize;
            canPasteSerializedData += (data) => true;
            unserializeAndPaste += OnPaste;
        }
        private void AddBlackBoard()
        {
            var blackboard = new AdvancedBlackBoard(this, View, new AdvancedBlackBoard.BlackBoardSettings()
            {
                showIsExposed = true,
                showIsGlobalToggle = true,
            });
            blackboard.SetPosition(new Rect(10, 100, 300, 400));
            Add(blackboard);
            BlackBoard = blackboard;
        }
        private void AddManipulators()
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
            this.AddManipulator(new GameObjectManipulator());
            this.AddManipulator(new JsonManipulator());
            this.AddManipulator(new ScriptableObjectManipulator());
        }
        private string OnSerialize(IEnumerable<GraphElement> elements)
        {
            CopyPaste.Copy(EditorWindow.GetInstanceID(), elements);
            return string.Empty;
        }
        private void OnPaste(string a, string b)
        {
            if (CopyPaste.CanPaste)
                Paste(new Vector2(50, 50));
        }
        private void Paste(Vector2 positionOffSet)
        {
            ClearSelection();
            //Add paste elements to selection
            foreach (var element in new CopyPasteGraphConvertor(this, CopyPaste.Paste(), positionOffSet).GetCopyElements())
            {
                element.Select(this, true);
            }
        }
        public IBehaviorTreeNode DuplicateNode(IBehaviorTreeNode node)
        {
            var newNode = nodeResolver.Create(node.GetBehavior(), this);
            Rect newRect = node.GetWorldPosition();
            newRect.position += new Vector2(50, 50);
            newNode.View.SetPosition(newRect);
            newNode.OnSelectAction = OnNodeSelect;
            AddElement(newNode.View);
            newNode.CopyFrom(node);
            return newNode;
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
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Paste", (evt) =>
            {
                Paste(contentViewContainer.WorldToLocal(evt.eventInfo.mousePosition) - CopyPaste.CenterPosition);
            }, x => CopyPaste.CanPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled));
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return PortHelper.GetCompatiblePorts(View, startAnchor);
        }
        /// <summary>
        /// Copy graph view nodes from other tree
        /// </summary>
        /// <param name="tree>
        /// <param name="mousePosition"></param>
        public void CopyFromTree(BehaviorTree tree, Vector2 mousePosition)
        {
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition) - new Vector2(400, 300);
            IEnumerable<IBehaviorTreeNode> nodes;
            RootNode rootNode;
            AddSharedVariablesToBlackBoard(tree, false);
            (rootNode, nodes) = converter.ConvertToNode(tree, this, localMousePosition);
            foreach (var node in nodes) node.OnSelectAction = OnNodeSelect;
            var edge = rootNode.Child.connections.First();
            RemoveElement(edge);
            RemoveElement(rootNode);
            RestoreBlocks(tree, nodes);
        }
        /// <summary>
        /// Serialize current editing behavior tree to json format
        /// </summary>
        /// <returns></returns>
        public string SerializeTreeToJson()
        {
            return BehaviorTree.Serialize(Instance, false, true);
        }
        /// <summary>
        /// Copy BehaviorTree from json file
        /// </summary>
        /// <param name="serializedData">json data</param>
        /// <param name="mousePosition">drag or init position</param>
        /// <returns></returns>
        public bool CopyFromJson(string serializedData, Vector3 mousePosition)
        {
            try
            {
                var temp = BehaviorTree.Deserialize(serializedData);
                CopyFromTree(temp, mousePosition);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Restore graph view nodes from editing behavior tree
        /// </summary>
        public void Restore()
        {
            OnRestore(Instance);
        }
        /// <summary>
        /// Restore behavior tree to the graph view nodes
        /// </summary>
        /// <param name="treeContainer>
        protected virtual void OnRestore(BehaviorTree tree)
        {
            AddSharedVariablesToBlackBoard(tree, true);
            IEnumerable<IBehaviorTreeNode> nodes;
            (root, nodes) = converter.ConvertToNode(tree, this, Vector2.zero);
            foreach (var node in nodes) node.OnSelectAction = OnNodeSelect;
            RestoreBlocks(tree, nodes);
        }
        private void AddSharedVariablesToBlackBoard(BehaviorTree tree, bool duplicateWhenConflict)
        {
            foreach (var variable in tree.SharedVariables)
            {
                if (!duplicateWhenConflict && SharedVariables.Any(x => x.Name == variable.Name)) continue;
                //In play mode, use original variable to observe value change
                if (Application.isPlaying)
                {
                    BlackBoard.AddSharedVariable(variable);
                }
                else
                {
                    BlackBoard.AddSharedVariable(variable.Clone());
                }
            }
        }
        private void RestoreBlocks(BehaviorTree tree, IEnumerable<IBehaviorTreeNode> nodes)
        {
            foreach (var nodeBlockData in tree.blockData)
            {
                this.CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.Guid)).Select(x => x.View));
            }
        }

        public bool Save()
        {
            if (Application.isPlaying) return false;
            if (Validate())
            {
                Commit(Container);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }

        public virtual bool Validate()
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
        internal void Commit(IBehaviorTreeContainer container)
        {
            OnCommit(container);
        }
        protected virtual void OnCommit(IBehaviorTreeContainer container)
        {
            Undo.RecordObject(container.Object, "Commit behavior tree change");
            var tree = new BehaviorTree();
            var stack = new Stack<IBehaviorTreeNode>();
            stack.Push(root);
            // save new components
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                node.Commit(stack);
            }
            root.PostCommit(tree);
            tree.variables = new List<SharedVariable>(SharedVariables);
            List<GroupBlock> NodeBlocks = graphElements.OfType<GroupBlock>().ToList();
            tree.blockData = new();
            foreach (var block in NodeBlocks)
            {
                block.Commit(tree.blockData);
            }
            container.SetBehaviorTreeData(tree.GetData());
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(container.Object);
        }
        public IReadOnlyList<ILayoutTreeNode> GetLayoutTreeChildren()
        {
            return new List<ILayoutTreeNode>() { root };
        }
    }
}