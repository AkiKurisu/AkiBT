using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
namespace Kurisu.AkiBT.Editor
{
    public abstract class BehaviorTreeNode : Node{

        protected NodeBehavior NodeBehavior {set;  get; }

        private Type dirtyNodeBehaviorType;
        public Type BehaviorType=>dirtyNodeBehaviorType;
        public Port Parent { private set; get; }
        
        private readonly VisualElement container;

        private readonly TextField description;
        public string Description=>description.value;
        private readonly FieldResolverFactory fieldResolverFactory;

        public readonly List<IFieldResolver> resolvers = new List<IFieldResolver>();
        public Action<BehaviorTreeNode> onSelectAction;
        protected BehaviorTreeView ownerTreeView;
        public override void OnSelected()
        {
            base.OnSelected();
            onSelectAction?.Invoke(this);
        }
        
        protected BehaviorTreeNode()
        {
            fieldResolverFactory = new FieldResolverFactory();
            container = new VisualElement();
            description = new TextField();
            Initialize();
        }

        private void Initialize()
        {
            AddDescription();
            mainContainer.Add(this.container);
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
            OnRestore();
        }
        public void CopyFrom(BehaviorTreeNode copyNode)
        {
            for(int i=0;i<copyNode.resolvers.Count;i++)
            {
                resolvers[i].Copy(copyNode.resolvers[i]);
            }
            description.value = copyNode.Description;
            NodeBehavior=Activator.CreateInstance(copyNode.GetBehavior()) as NodeBehavior;
            NodeBehavior.NotifyEditor = MarkAsExecuted;
        }

        protected virtual void OnRestore()
        {

        }

        public NodeBehavior ReplaceBehavior()
        {
            this.NodeBehavior = Activator.CreateInstance(GetBehavior()) as NodeBehavior;
            return NodeBehavior;
        }

        protected virtual void AddParent()
        {
            Parent = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
            Parent.portName = "Parent";
            inputContainer.Add(Parent);
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
            
        public void Commit(Stack<BehaviorTreeNode> stack)
        {
            OnCommit(stack);
            resolvers.ForEach( r => r.Commit(NodeBehavior));
            NodeBehavior.description = this.description.value;
            NodeBehavior.graphPosition = GetPosition();
            NodeBehavior.NotifyEditor = MarkAsExecuted;
        }
        protected abstract void OnCommit(Stack<BehaviorTreeNode> stack);

        public bool Validate(Stack<BehaviorTreeNode> stack)
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

        protected abstract bool OnValidate(Stack<BehaviorTreeNode> stack);
        /// <summary>
        ///  ??????:????????????????????????
        /// </summary>
        /// <param name="nodeBehavior"></param>
        public void SetBehavior(System.Type nodeBehavior,BehaviorTreeView ownerTreeView=null)
        {
            if(ownerTreeView!=null)this.ownerTreeView=ownerTreeView;
            if (dirtyNodeBehaviorType != null)
            {
                dirtyNodeBehaviorType = null;
                container.Clear();
                resolvers.Clear();
            }
            dirtyNodeBehaviorType = nodeBehavior;

            nodeBehavior
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null)//??????Atrribute????????????????????????
                .Concat(GetAllFields(nodeBehavior))//Concat????????????
                .Where(field => field.IsInitOnly == false)
                .ToList().ForEach((p) =>
                {
                    var fieldResolver = fieldResolverFactory.Create(p);//????????????????????????
                    var defaultValue = Activator.CreateInstance(nodeBehavior) as NodeBehavior;
                    fieldResolver.Restore(defaultValue);
                    container.Add(fieldResolver.GetEditorField(ownerTreeView));
                    resolvers.Add(fieldResolver);
                });
            AkiLabel[] array;
            if ((array = (nodeBehavior.GetCustomAttributes(typeof(AkiLabel), false) as AkiLabel[])).Length > 0)
            {
                title = array[0].Title;
            }
            else
                title = nodeBehavior.Name;
            styleSheets.Add((StyleSheet)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/AkiBT/Node.uss", typeof(StyleSheet)));
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<SerializeField>() != null)
                .Where(field => field.GetCustomAttribute<HideInEditorWindow>() == null).Concat(GetAllFields(t.BaseType));//Concat????????????
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

       
    }
}