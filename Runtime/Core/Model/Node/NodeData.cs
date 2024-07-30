using System;
using System.Reflection;
using UnityEngine;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Class store the node data, editor only
    /// </summary>
    [Serializable]
    public class NodeData
    {
        [Serializable]
        public struct NodeType
        {
            public string _class;
            public string _ns;
            public string _asm;
            public NodeType(string _class, string _ns, string _asm)
            {
                this._class = _class;
                this._ns = _ns;
                this._asm = _asm;
            }
            public NodeType(Type type)
            {
                _class = type.Name;
                _ns = type.Namespace;
                _asm = type.Assembly.GetName().Name;
            }
            public readonly Type ToType()
            {
                return Type.GetType(Assembly.CreateQualifiedName(_asm, $"{_ns}.{_class}"));
            }
            public override readonly string ToString()
            {
                return $"class:{_class} ns: {_ns} asm:{_asm}";
            }
        }
        public Rect graphPosition = new(400, 300, 100, 100);
        public string description;
        public string guid;
        // metadata to find missing class or recover
        public NodeType nodeType;
        public string serializedData;
        public void Serialize(NodeBehavior nodeBehavior)
        {
            nodeType = new NodeType(nodeBehavior.GetType());
            serializedData = BehaviorTreeData.SmartSerialize(nodeBehavior, verbose: false);
        }
        public NodeData Clone()
        {
            return new NodeData
            {
                graphPosition = graphPosition,
                description = description,
                guid = guid,
                nodeType = nodeType,
                serializedData = serializedData
            };
        }
    }
}