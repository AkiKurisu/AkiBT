using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeView: GraphView
    {
        private readonly struct EdgePair
        {
            public readonly NodeBehavior NodeBehavior;
            public readonly Port ParentOutputPort;

            public EdgePair(NodeBehavior nodeBehavior, Port parentOutputPort)
            {
                NodeBehavior = nodeBehavior;
                ParentOutputPort = parentOutputPort;
            }
        }

        private readonly BehaviorTree behaviorTree;
        public Root mRoot=>behaviorTree.Root;
        private RootNode root;
        public bool AutoSave
        {
            get=>behaviorTree.AutoSave;
            set=>behaviorTree.AutoSave=value;
        }
        public string SavePath
        {
            get=>behaviorTree.SavePath;
            set=>behaviorTree.SavePath=value;
        }
        private readonly NodeResolver nodeResolver = new NodeResolver();
        /// <summary>
        /// 结点选择委托
        /// </summary>
        public System.Action<BehaviorTreeNode> onSelectAction;  
        /// <summary>
        /// 黑板
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeView(BehaviorTree bt, EditorWindow editor)
        {
            behaviorTree = bt;
            style.flexGrow = 1;
            style.flexShrink = 1;
            styleSheets.Add(Resources.Load<StyleSheet>("Graph"));
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

            var provider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            provider.Initialize(this, editor);

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), provider);
            };
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
        /// <summary>
        /// 重铸行为树
        /// </summary>
        public void Restore()
        {
            var stack = new Stack<EdgePair>();
            if(behaviorTree.ExternalBehaviorTree!=null)
                stack.Push(new EdgePair(behaviorTree.ExternalBehaviorTree.Root, null));
            else
                stack.Push(new EdgePair(behaviorTree.Root, null));
            while (stack.Count > 0)
            {
                // create node
                var edgePair = stack.Pop();
                if (edgePair.NodeBehavior == null)
                {
                    continue;
                }
                var node = nodeResolver.CreateNodeInstance(edgePair.NodeBehavior.GetType());
                node.Restore(edgePair.NodeBehavior);
                /// <summary>
                /// 添加选中委托
                /// </summary>
                node.onSelectAction=onSelectAction;
                AddElement(node);
                node.SetPosition( edgePair.NodeBehavior.graphPosition);

                // connect parent
                if (edgePair.ParentOutputPort != null)
                {
                    var edge = ConnectPorts(edgePair.ParentOutputPort, node.Parent);
                    AddElement(edge);
                }

                // seek child
                switch (edgePair.NodeBehavior)
                {
                    case Composite nb:
                    {
                        var compositeNode = node as CompositeNode;
                        var addible = nb.Children.Count - compositeNode.ChildPorts.Count;
                        for (var i = 0; i < addible; i++)
                        {
                            compositeNode.AddChild();
                        }

                        for (var i = 0; i < nb.Children.Count; i++)
                        {
                            stack.Push(new EdgePair(nb.Children[i], compositeNode.ChildPorts[i]));
                        }
                        break;
                    }
                    case Conditional nb:
                    {
                        var conditionalNode = node as ConditionalNode;
                        stack.Push(new EdgePair(nb.Child, conditionalNode.Child));
                        break;
                    }
                    case Decorator nb:
                    {
                        var decoratorNode = node as DecoratorNode;
                        stack.Push(new EdgePair(nb.Child, decoratorNode.Child));
                        break;
                    }
                    case Root nb:
                    {
                        root = node as RootNode;
                        if (nb.Child != null)
                        {
                            stack.Push(new EdgePair(nb.Child, root.Child));
                        }
                        break;
                    }
                }
            }
        }


        public bool Save(bool autoSave=false)
        {
            if(Application.isPlaying)return false;
            if (Validate())
            {
                Commit();
                if(autoSave)
                    Debug.Log($"<color=#3aff48>AkiBT</color>自动保存成功{System.DateTime.Now.ToString()}");
                return true;
            }
            if(autoSave)
                Debug.Log($"<color=#ff2f2f>AkiBT</color>自动保存失败{System.DateTime.Now.ToString()}");
            return false;
        }

        private bool Validate()
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

        private void Commit()
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
            
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(behaviorTree);
        }
        public void Commit(BehaviorTreeSO treeSO)
        {
            var stack = new Stack<BehaviorTreeNode>();
            stack.Push(root);
            
            // save new components
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                node.Commit(stack);
            }
            
            root.PostCommit(treeSO);
            
            // notify to unity editor that the tree is changed.
            EditorUtility.SetDirty(treeSO);
        }
        private static Edge ConnectPorts(Port output, Port input)
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

    }
}