using UnityEngine;
using System.Linq;
using System;
using UObject = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
#endif
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Expandable tree structure to solve class missing cause children loss
    /// </summary>
    [Serializable]
    public class BehaviorTreeData
    {
        [Serializable]
        public class Edge
        {
            public int[] children;
        }
        [SerializeReference]
        public SharedVariable[] variables;
        [SerializeReference]
        public NodeBehavior[] behaviors;
        public Edge[] edges;
#if UNITY_EDITOR
        [SerializeField]
        private NodeData[] nodeData;
        [SerializeField]
        internal GroupBlockData[] blockData;
#endif
        public BehaviorTreeData() { }
        public BehaviorTreeData(BehaviorTree tree)
        {
            variables = tree.variables.ToArray();
            behaviors = tree.ToArray();
            edges = new Edge[behaviors.Length];
#if UNITY_EDITOR
            nodeData = new NodeData[behaviors.Length];
#endif
            for (int i = 0; i < behaviors.Length; ++i)
            {
#if UNITY_EDITOR
                nodeData[i] = behaviors[i].nodeData;
#endif
                var edge = edges[i] = new Edge();
                edge.children = new int[behaviors[i].GetChildrenCount()];
                for (int n = 0; n < edge.children.Length; ++n)
                {
                    edge.children[n] = Array.IndexOf(behaviors, behaviors[i].GetChildAt(n));
                }
                // clear duplicated reference
                behaviors[i].ClearChildren();
            }
            blockData = tree.blockData.ToArray();
        }
        public NodeBehavior Build()
        {
            if (edges == null || edges.Length == 0) return null;
            if (behaviors == null || behaviors.Length == 0) return null;
            if (behaviors.Length != edges.Length)
            {
                throw new ArgumentException("The length of behaviors and edges must be the same.");
            }
            for (int n = 0; n < behaviors.Length; ++n)
            {
                var edge = edges[n];
                var behavior = behaviors[n];
#if UNITY_EDITOR
                if (nodeData != null && nodeData.Length > n)
                    behavior.nodeData = nodeData[n];
#endif
                for (int i = 0; i < edge.children.Length; i++)
                {
                    int childIndex = edge.children[i];
                    if (childIndex >= 0 && childIndex < behaviors.Length)
                    {
                        var child = behaviors[childIndex];
                        // use invalid node to replace missing nodes
                        if (child == null)
                        {
                            if (edges[childIndex].children.Length > 0)
                            {
                                child = new InvalidComposite();
                            }
                            else
                            {
                                child = new InvalidAction();
                            }
                        }
                        behavior.AddChild(child);
                    }
                }
            }
            return behaviors[0];
        }
        public BehaviorTreeData Clone()
        {
            // use internal serialization to solve UObject hard reference
            return JsonUtility.FromJson<BehaviorTreeData>(JsonUtility.ToJson(this));
        }
        public BehaviorTree CreateInstance()
        {
            return new BehaviorTree(Clone());
        }
        /// <summary>
        /// Deserialize from json, but has limit at runtime
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        public static BehaviorTreeData Deserialize(string serializedData)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(serializedData))
            {
                JObject obj = JObject.Parse(serializedData);
                foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
                {
                    if (prop.Name == "instanceID")
                    {
                        var UObject = AssetDatabase.LoadAssetAtPath<UObject>(AssetDatabase.GUIDToAssetPath((string)prop.Value));
                        if (UObject == null)
                        {
                            prop.Value = 0;
                            continue;
                        }
                        prop.Value = UObject.GetInstanceID();
                    }
                }
                return JsonUtility.FromJson<BehaviorTreeData>(obj.ToString(Formatting.Indented));
            }
#endif
            return JsonUtility.FromJson<BehaviorTreeData>(serializedData);
        }
        /// <summary>
        /// Serialize to json, but has limit at runtime
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="indented"></param>
        /// <param name="serializeEditorData"></param>
        /// <returns></returns>
        public static string Serialize(BehaviorTreeData tree, bool indented = false, bool serializeEditorData = false)
        {
            if (tree == null) return null;
            var json = JsonUtility.ToJson(tree);
#if UNITY_EDITOR
            JObject obj = JObject.Parse(json);
            foreach (JProperty prop in obj.Descendants().OfType<JProperty>().ToList())
            {
                //Remove editor only fields in behaviorTree manually
                if (!serializeEditorData)
                    if (prop.Name == "graphPosition" || prop.Name == "description" || prop.Name == "guid")
                    {
                        prop.Remove();
                    }
                if (prop.Name == "instanceID")
                {
                    string propertyName = prop.Name;
                    if (prop.Parent?.Parent != null) propertyName = (prop.Parent?.Parent as JProperty).Name;
                    var UObject = EditorUtility.InstanceIDToObject((int)prop.Value);
                    if (UObject == null) continue;
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UObject));
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogWarning($"<color=#fcbe03>AkiBT</color>: Can't serialize UnityEngine.Object field {propertyName}");
                        continue;
                    }
                    //Convert to GUID
                    prop.Value = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UObject));
                }
            }
            return obj.ToString(indented ? Formatting.Indented : Formatting.None);
#else
            return json;
#endif
        }
    }
}