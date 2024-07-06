using System;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Class store the node data in the tree hierarchy, editor only
    /// </summary>
    [Serializable]
    public class NodeData
    {
        // TODO: add meta data to find missing class or recover
        public Rect graphPosition = new(400, 300, 100, 100);
        public string description;
        public string guid;
        public NodeData Clone()
        {
            return new NodeData
            {
                graphPosition = graphPosition,
                description = description,
                guid = guid
            };
        }
    }
}