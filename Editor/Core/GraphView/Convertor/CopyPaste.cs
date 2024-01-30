using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class CopyPaste
    {
        private static Graph graph;
        /// <summary>
        /// Copy elements to global without serialization
        /// </summary>
        /// <param name="instanceID">EditorWindow InstanceID</param>
        /// <param name="elements"></param>
        public static void Copy(int instanceID, IEnumerable<GraphElement> elements)
        {
            graph = new Graph()
            {
                instanceID = instanceID,
                edges = elements.OfType<Edge>().ToArray(),
                nodes = elements.OfType<Node>().ToArray()
            };
        }
        public static List<GraphElement> Paste()
        {
            var list = new List<GraphElement>();
            if (graph != null)
            {
                list.AddRange(graph.edges);
                list.AddRange(graph.nodes);
            }
            return list;
        }
        public static bool CanPaste => graph != null && GraphEditorWindow.ContainsEditorWindow(graph.instanceID);
        public static Vector2 CenterPosition
        {
            get
            {
                if (graph == null) return Vector2.zero;
                var average = Vector2.zero;
                int count = 0;
                foreach (var node in graph.nodes)
                {
                    if (node is not IBehaviorTreeNode behaviorTreeNode) continue;
                    count++;
                    average += behaviorTreeNode.GetWorldPosition().position;
                }
                return average / count;
            }
        }
        private class Graph
        {
            public int instanceID;
            public Edge[] edges;
            public Node[] nodes;
        }
    }
}