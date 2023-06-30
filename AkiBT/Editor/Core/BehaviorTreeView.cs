using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;
using System;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeView: GraphView,ITreeView
    {
        public Blackboard _blackboard;
        protected readonly IBehaviorTree behaviorTree;
        public IBehaviorTree BehaviorTree=>behaviorTree;
        protected RootNode root;
        private List<SharedVariable> exposedProperties=new List<SharedVariable>();
        public List<SharedVariable> ExposedProperties=>exposedProperties;
        private FieldResolverFactory fieldResolverFactory = FieldResolverFactory.Instance;
        protected NodeSearchWindowProvider provider;
        public event System.Action<SharedVariable> OnPropertyNameChangeEvent;
        public event System.Action<SharedVariable> OnPropertyNameEditingEvent;
        internal static bool AutoSave
        {
            get=>setting.AutoSave;
            set=>setting.AutoSave=value;
        }
        internal static string SavePath
        {
            get=>setting.SavePath;
            set=>setting.SavePath=value;
        }
        /// <summary>
        /// 是否可以保存为SO,如果可以会在ToolBar中提供按钮
        /// </summary>
        public virtual bool CanSaveToSO=>behaviorTree is BehaviorTree;
        public virtual string TreeEditorName=>"AkiBT";
        private readonly NodeResolver nodeResolver = new NodeResolver();
        /// <summary>
        /// 结点选择委托
        /// </summary>
        public System.Action<BehaviorTreeNode> onSelectAction;  
        private readonly EditorWindow _window;
        public bool IsRestored{get;private set;}
        private readonly BehaviorNodeConverter converter=new BehaviorNodeConverter();
        private readonly DragDropManipulator dragDropManipulator;
        private static readonly BehaviorTreeSetting setting=BehaviorTreeSetting.GetOrCreateSettings();
        /// <summary>
        /// 黑板
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeView(IBehaviorTree bt, EditorWindow editor)
        {
            _window=editor;
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
            dragDropManipulator=new DragDropManipulator(this);
            dragDropManipulator.OnDragOverEvent+=CopyFromObject;
            this.AddManipulator(dragDropManipulator);
            provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, editor,BehaviorTreeSetting.GetMask(TreeEditorName));

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
            serializeGraphElements += CopyOperation;
            canPasteSerializedData+=(data)=>true;
            unserializeAndPaste+=OnPaste;
        }
        private string CopyOperation(IEnumerable<GraphElement> elements)
        {
            ClearSelection();
            foreach (GraphElement n in elements)
            {
                if(n is BehaviorTreeNode&&!(n as BehaviorTreeNode).Copiable)continue;
                AddToSelection(n);
            }
            return "Copy Nodes";
        }
        /// <summary>
        /// 粘贴结点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void OnPaste(string a,string b)
        {
            List<ISelectable> copyElements=new CopyPasteGraph(this,selection).GetCopyElements();
            ClearSelection();
            //再次选择
            copyElements.ForEach(node=>
            {
                node.Select(this,true);
            });
        }
        public BehaviorTreeNode DuplicateNode(BehaviorTreeNode node)
        {
            var newNode =nodeResolver.CreateNodeInstance(node.GetBehavior(),this) as BehaviorTreeNode;
            Rect newRect=node.GetPosition();
            newRect.position+=new Vector2(50,50);
            newNode.SetPosition(newRect);
            newNode.onSelectAction=onSelectAction;
            AddElement(newNode);
            newNode.CopyFrom(node);
            return newNode;
        }
        public GroupBlock CreateBlock(Rect rect, GroupBlockData blockData = null)
        {
            if(blockData==null)blockData = new GroupBlockData();
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
            OnPropertyNameEditingEvent?.Invoke(variable);
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var remainTargets = evt.menu.MenuItems().FindAll(e =>
            {
                switch (e)
                {
                    case BehaviorTreeDropdownMenuAction _ : return true;
                    case DropdownMenuAction a: return a.name == "Create Node" || a.name == "Delete";
                    default: return false;
                }
            });
            //Remove needless default actions .
            evt.menu.MenuItems().Clear();
            remainTargets.ForEach(evt.menu.MenuItems().Add);
        }
        public void AddPropertyToBlackBoard(SharedVariable variable)
        {
            var localPropertyValue = variable.GetValue();
            if(String.IsNullOrEmpty(variable.Name))variable.Name=variable.GetType().Name;
            var localPropertyName = variable.Name;
            int index=1;
            while (ExposedProperties.Any(x => x.Name == localPropertyName))
            {
                localPropertyName = $"{variable.Name}{index}";
                index++;
            }
            variable.Name=localPropertyName;
            ExposedProperties.Add(variable);
            var container = new VisualElement();
            var field = new BlackboardField {text = localPropertyName, typeText = variable.GetType().Name};
            field.capabilities &=~Capabilities.Deletable;
            field.capabilities &=~Capabilities.Movable;
            container.Add(field);
            FieldInfo info=variable.GetType().GetField("value",BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Public);
            var fieldResolver = fieldResolverFactory.Create(info);
            var valueField=fieldResolver.GetEditorField(ExposedProperties,variable);
            var sa = new BlackboardRow(field, valueField);
            sa.AddManipulator(new ContextualMenuManipulator((evt)=>BuildBlackboardMenu(evt,container,variable)));
            container.Add(sa);
            _blackboard.Add(container);
        }
        private void BuildBlackboardMenu(ContextualMenuPopulateEvent evt,VisualElement element,SharedVariable variable)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Delate Variable", (a) =>
            {
                   int index=_blackboard.contentContainer.IndexOf(element);
                   ExposedProperties.RemoveAt(index-1);
                   _blackboard.Remove(element);
                   return;
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Update All Reference Node", (a) =>
            {
                   OnPropertyNameChangeEvent?.Invoke(variable);
            }));
        }
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter) {
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
        private void CopyFromObject(UnityEngine.Object data,Vector3 mousePosition)
        {
            if(data is GameObject)
            {
                if((data as GameObject).TryGetComponent<IBehaviorTree>(out IBehaviorTree tree))
                {
                    _window.ShowNotification(new GUIContent("GameObject Dropped Successfully!"));
                    CopyFromOtherTree(tree,mousePosition);
                    return;
                }
                _window.ShowNotification(new GUIContent("Invalid Drag GameObject!"));
                return;
            }
            if(data is not IBehaviorTree)
            {
                _window.ShowNotification(new GUIContent("Invalid Drag Data!"));
                return;
            }
            _window.ShowNotification(new GUIContent("Data Dropped Successfully!"));
            CopyFromOtherTree(data as IBehaviorTree,mousePosition);
        }
        internal protected void CopyFromOtherTree(IBehaviorTree otherTree,Vector2 mousePosition)
        {
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition)-new Vector2(400,300);
            IsRestored=true;
            IEnumerable<BehaviorTreeNode>nodes;
            RootNode rootNode;
            foreach(var variable in otherTree.SharedVariables)
            {
                AddPropertyToBlackBoard(variable.Clone() as SharedVariable);//Clone新的共享变量
            } 
            (rootNode,nodes)=converter.ConvertToNode(otherTree,this,localMousePosition);
            foreach(var node in nodes)node.onSelectAction=onSelectAction;
            var edge=rootNode.Child.connections.First();
            RemoveElement(edge);
            RemoveElement(rootNode);
            foreach (var nodeBlockData in otherTree.BlockData)
            {
               CreateBlock(new Rect(nodeBlockData.Position,  new Vector2(100, 100)),nodeBlockData)
               .AddElements(nodes.Where(x=>nodeBlockData.ChildNodes.Contains(x.GUID)));
            }
            IsRestored=false;
        }
        internal void Restore()
        {
            IsRestored=true;
            IBehaviorTree tree=behaviorTree.ExternalBehaviorTree??behaviorTree;
            OnRestore(tree);
            IsRestored=false;
        }
        protected virtual void OnRestore(IBehaviorTree tree)
        {
            RestoreSharedVariables(tree);
            IEnumerable<BehaviorTreeNode>nodes;
            (this.root,nodes)=converter.ConvertToNode(tree,this,Vector2.zero);
            foreach(var node in nodes)node.onSelectAction=onSelectAction;
            RestoreBlocks(tree,nodes);
        }
        protected void RestoreSharedVariables(IBehaviorTree tree)
        {
            foreach(var variable in tree.SharedVariables)
            {
                AddPropertyToBlackBoard(variable.Clone() as SharedVariable);//Clone新的共享变量
            }
        }
        protected void RestoreBlocks(IBehaviorTree tree,IEnumerable<BehaviorTreeNode>nodes)
        {
            foreach (var nodeBlockData in tree.BlockData)
            {
               CreateBlock(new Rect(nodeBlockData.Position,  new Vector2(100, 100)),nodeBlockData)
               .AddElements(nodes.Where(x=>nodeBlockData.ChildNodes.Contains(x.GUID)));
            }
        }
        void ITreeView.SelectGroup(BehaviorTreeNode node)
        {
            var block =CreateBlock(new Rect(node.transform.position,  new Vector2(100, 100)));
            foreach(var select in selection)
            {
                if(select is not BehaviorTreeNode||select is RootNode)continue;
                block.AddElement(select as BehaviorTreeNode);
            }
        }
        void ITreeView.UnSelectGroup()
        {
            foreach(var select in selection)
            {
                if(select is not BehaviorTreeNode)continue;
                var node=select as BehaviorTreeNode;
                var block=graphElements.ToList().Where(x => x is GroupBlock)?.Cast<GroupBlock>()?.FirstOrDefault(x=>x.ContainsElement(node));
                if(block!=null)block.RemoveElement(node);
            }
        }

        internal bool Save(bool autoSave=false)
        {
            if(Application.isPlaying)return false;
            if (Validate())
            {
                Commit(behaviorTree);
                if(autoSave)Debug.Log($"<color=#3aff48>{TreeEditorName}</color>[{behaviorTree._Object.name}]自动保存成功{System.DateTime.Now.ToString()}");
                AssetDatabase.SaveAssets();
                return true;
            }
            if(autoSave)Debug.Log($"<color=#ff2f2f>{TreeEditorName}</color>[{behaviorTree._Object.name}]自动保存失败{System.DateTime.Now.ToString()}");
            return false;
        }

        protected virtual bool Validate()
        {
            //validate nodes by DFS.
            var stack = new Stack<BehaviorTreeNode>();
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
            var stack = new Stack<BehaviorTreeNode>();
            stack.Push(root);
            // save new components
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                node.Commit(stack);
            }
            root.PostCommit(behaviorTree);
            behaviorTree.SharedVariables.Clear();
            foreach(var sharedVariable in ExposedProperties)
            {
                behaviorTree.SharedVariables.Add(sharedVariable);
            }
            List<GroupBlock> NodeBlocks =graphElements.ToList().Where(x => x is GroupBlock).Cast<GroupBlock>().ToList();
            behaviorTree.BlockData.Clear();
            foreach (var block in NodeBlocks)
            {
                var nodes = block.containedElements.Where(x => x is BehaviorTreeNode)
                .Cast<BehaviorTreeNode>()
                .Select(x => x.GUID)
                .ToList();
                behaviorTree.BlockData.Add(new GroupBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
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
            return BehaviorTreeSerializeUtility.SerializeTree(behaviorTree,false,true);
        }
        internal bool CopyFromJsonFile(string serializedData)
        {
            var temp=ScriptableObject.CreateInstance<BehaviorTreeSO>();
            try
            {
                temp.Deserialize(serializedData);
                CopyFromOtherTree(temp,Vector2.zero);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}