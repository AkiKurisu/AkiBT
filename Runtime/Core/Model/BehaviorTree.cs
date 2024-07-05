using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;
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
            SharedVariableMapper.Traverse(this);
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
            return new TraverseIterator(root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TraverseIterator(root);
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
    }
}