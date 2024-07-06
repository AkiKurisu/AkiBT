using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;
using UnityEngine.Pool;
namespace Kurisu.AkiBT
{
    [Serializable]
    public class BehaviorTree : IVariableSource, IEnumerable<NodeBehavior>
    {
        [SerializeReference]
        internal List<SharedVariable> variables;
        [SerializeReference]
        internal Root root;
        public Root Root => root;
        public List<SharedVariable> SharedVariables => variables;
#if UNITY_EDITOR
        [SerializeField]
        internal List<GroupBlockData> blockData = new();
#endif
        public BehaviorTree() { }
        public BehaviorTree(BehaviorTreeData behaviorTreeData)
        {
            variables = behaviorTreeData.variables.ToList();
            root = behaviorTreeData.Build() as Root;
            root ??= new Root();
#if UNITY_EDITOR
            blockData = behaviorTreeData.blockData.ToList();
#endif
        }
        /// <summary>
        /// Initialize behavior tree's shared variables
        /// </summary>
        /// <param name="bindToGlobal">Whether bind properties assigned with isGlobal to global variables</param>
        public void InitVariables(bool bindToGlobal = true)
        {
            SharedVariableHelper.InitVariables(this);
            if (bindToGlobal) this.MapGlobal();
        }
        public void Run(GameObject gameObject)
        {
            root.Run(gameObject, this);
        }
        public void Awake()
        {
            root.Awake();
        }
        public void Start()
        {
            root.Start();
        }
        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }
        public Status TickWithStatus()
        {
            root.PreUpdate();
            var status = root.Update();
            root.PostUpdate();
            return status;
        }
        public void Abort()
        {
            root.Abort();
        }

        public IEnumerator<NodeBehavior> GetEnumerator()
        {
            return new Enumerator(root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(root);
        }
        public BehaviorTree Clone()
        {
            // use internal serialization to solve UObject hard reference
            return JsonUtility.FromJson<BehaviorTree>(JsonUtility.ToJson(this));
        }
        public BehaviorTreeData GetData()
        {
            return new BehaviorTreeData(Clone());
        }
        public static BehaviorTree Deserialize(string serializedData)
        {
            // not cache behavior tree data!
            return BehaviorTreeData.Deserialize(serializedData).CreateInstance();
        }
        public string Serialize(bool indented = false, bool serializeEditorData = false)
        {
            return Serialize(this, indented, serializeEditorData);
        }
        public static string Serialize(BehaviorTree tree, bool indented = false, bool serializeEditorData = false)
        {
            if (tree == null) return null;
            return BehaviorTreeData.Serialize(tree.GetData(), indented, serializeEditorData);
        }
        private struct Enumerator : IEnumerator<NodeBehavior>
        {
            private readonly Stack<NodeBehavior> stack;
            private static readonly ObjectPool<Stack<NodeBehavior>> pool = new(() => new(), null, s => s.Clear());
            private NodeBehavior currentNode;
            public Enumerator(NodeBehavior root)
            {
                stack = pool.Get();
                currentNode = null;
                if (root != null)
                {
                    stack.Push(root);
                }
            }

            public readonly NodeBehavior Current
            {
                get
                {
                    if (currentNode == null)
                    {
                        throw new InvalidOperationException();
                    }
                    return currentNode;
                }
            }

            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                pool.Release(stack);
                currentNode = null;
            }
            public bool MoveNext()
            {
                if (stack.Count == 0)
                {
                    return false;
                }

                currentNode = stack.Pop();
                int childrenCount = currentNode.GetChildrenCount();
                for (int i = childrenCount - 1; i >= 0; i--)
                {
                    stack.Push(currentNode.GetChildAt(i));
                }
                return true;
            }
            public void Reset()
            {
                stack.Clear();
                if (currentNode != null)
                {
                    stack.Push(currentNode);
                }
                currentNode = null;
            }
        }
    }
}