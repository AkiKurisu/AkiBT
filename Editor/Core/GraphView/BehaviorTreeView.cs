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
        public IBlackBoard BlackBoard { get; protected set; }
        protected readonly IBehaviorTree behaviorTree;
        public IBehaviorTree BehaviorTree => behaviorTree;
        protected RootNode root;
        public List<SharedVariable> SharedVariables { get; } = new();
        public virtual string TreeEditorName => "AkiBT";
        private readonly NodeResolverFactory nodeResolver = NodeResolverFactory.Instance;
        public Action<IBehaviorTreeNode> OnNodeSelect { get; set; }
        public EditorWindow EditorWindow { get; internal set; }
        private readonly BehaviorNodeConvertor converter = new();
        VisualElement ILayoutTreeNode.View => this;
        public GraphView View => this;
        public IControlGroupBlock GroupBlockController { get; protected set; }
        public BehaviorTreeView(IBehaviorTree bt, EditorWindow editor)
        {
            EditorWindow = editor;
            behaviorTree = bt;
            ConstructTreeView();
        }
        /// <summary>
        /// Build tree view elements
        /// </summary>
        protected virtual void ConstructTreeView()
        {
            SetStyle();
            AddManipulators();
            AddNodeProvider();
            AddGroupBlockController();
            RegisterSerializationCallBack();
            AddBlackBoard();
        }
        private void SetStyle()
        {
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(BehaviorTreeSetting.GetGraphStyle(TreeEditorName));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new GridBackground());
        }
        private void AddNodeProvider()
        {
            var provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, BehaviorTreeSetting.GetMask(TreeEditorName));
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
        }
        private void AddGroupBlockController()
        {
            GroupBlockController = new GroupBlockController(this);
        }
        private void RegisterSerializationCallBack()
        {
            canPasteSerializedData += (data) => true;
            unserializeAndPaste += OnPaste;
        }
        private void AddBlackBoard()
        {
            var blackboard = new AdvancedBlackBoard(this, View);
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
            this.AddManipulator(new DragDropManipulator(CopyFromObject));
        }
        private void OnPaste(string a, string b)
        {
            List<ISelectable> copyElements = new CopyPasteConvertor(this, selection).GetPasteElements();
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
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return PortHelper.GetCompatiblePorts(View, startAnchor);
        }
        /// <summary>
        /// Copy graph view nodes from UObject
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mousePosition"></param>
        public void CopyFromObject(UnityEngine.Object data, Vector2 mousePosition)
        {
            if (data is GameObject gameObject)
            {
                if (gameObject.TryGetComponent(out IBehaviorTree tree))
                {
                    EditorWindow.ShowNotification(new GUIContent("GameObject Dropped Succeed !"));
                    CopyFromTree(tree, mousePosition);
                    return;
                }
                EditorWindow.ShowNotification(new GUIContent("Invalid Drag GameObject !"));
                return;
            }
            if (data is TextAsset asset)
            {
                if (CopyFromJson(asset.text, mousePosition))
                    EditorWindow.ShowNotification(new GUIContent("Text Asset Dropped Succeed !"));
                else
                    EditorWindow.ShowNotification(new GUIContent("Invalid Drag Text Asset !"));
                return;
            }
            if (data is not IBehaviorTree)
            {
                EditorWindow.ShowNotification(new GUIContent("Invalid Drag Data !"));
                return;
            }
            EditorWindow.ShowNotification(new GUIContent("Data Dropped Succeed !"));
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
            foreach (var node in nodes) node.OnSelectAction = OnNodeSelect;
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
            return SerializeUtility.SerializeTree(behaviorTree, false, true);
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
        /// <summary>
        /// Restore graph view nodes from editing behavior tree
        /// </summary>
        public void Restore()
        {
            if (!Application.isPlaying && BehaviorTreeEditorUtility.TryGetExternalTree(behaviorTree, out IBehaviorTree tree))
            {
                OnRestore(tree);
            }
            else
            {
                OnRestore(behaviorTree);
            }
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
            foreach (var node in nodes) node.OnSelectAction = OnNodeSelect;
            RestoreBlocks(tree, nodes);
        }
        private void RestoreSharedVariables(IBehaviorTree tree)
        {
            foreach (var variable in tree.SharedVariables)
            {
                //In play mode, use original variable to observe value change
                if (Application.isPlaying)
                {
                    BlackBoard.AddSharedVariable(variable);
                }
                else
                {
                    BlackBoard.AddSharedVariable(variable.Clone() as SharedVariable);
                }
            }
        }
        private void RestoreBlocks(IBehaviorTree tree, IEnumerable<IBehaviorTreeNode> nodes)
        {
            foreach (var nodeBlockData in tree.BlockData)
            {
                GroupBlockController.CreateBlock(new Rect(nodeBlockData.Position, new Vector2(100, 100)), nodeBlockData)
                .AddElements(nodes.Where(x => nodeBlockData.ChildNodes.Contains(x.GUID)).Select(x => x.View));
            }
        }

        internal bool Save()
        {
            if (Application.isPlaying) return false;
            if (Validate())
            {
                Commit(behaviorTree);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }

        internal protected virtual bool Validate()
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
            foreach (var sharedVariable in SharedVariables)
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