using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
namespace Kurisu.AkiBT.Editor
{
    public class PortHelper
    {
        public static Edge ConnectPorts(Port output, Port input)
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
        public static IBehaviorTreeNode FindChildNode(Port port)
        {
            if (port.childCount == 0) return null;
            var child = port.connections.FirstOrDefault()?.input?.node;
            if (child == null) return null;
            else if (child is BehaviorTreeNode node)
            {
                return node;
            }
            else if (child is ParentBridge bridge)
            {
                return bridge.GetFirstAncestorOfType<CompositeStack>();
            }
            return null;
        }
        public static List<Port> GetCompatiblePorts(GraphView graphView, Port startAnchor)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in graphView.ports.ToList())
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
    }
}
