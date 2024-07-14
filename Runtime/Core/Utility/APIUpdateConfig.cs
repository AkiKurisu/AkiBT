using System;
using System.Linq;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [CreateAssetMenu(fileName = "APIUpdateConfig", menuName = "AkiBT/APIUpdateConfig")]
    public class APIUpdateConfig : ScriptableObject
    {
        [Serializable]
        public class SerializeType
        {
            private Type type;
            public Type Type => type ??= ToType();
            public string nodeType;
            private Type ToType()
            {
                var tokens = nodeType.Split(' ');
                return new NodeData.NodeType(tokens[0], tokens[1], tokens[2]).ToType();
            }
            public string GetFullTypeName()
            {
                return $"{Type.Assembly.GetName().Name} {Type.FullName}";
            }
            public SerializeType() { }
            public SerializeType(Type type)
            {
                NodeData.NodeType node = new(type);
                nodeType = $"{node._class} {node._ns} {node._asm}";
            }
            public SerializeType(NodeData.NodeType nodeType)
            {
                this.nodeType = $"{nodeType._class} {nodeType._ns} {nodeType._asm}";
            }
        }
        [Serializable]
        public class Pair
        {
            public SerializeType sourceType;
            public SerializeType targetType;
            public Pair() { }
            public Pair(Type sourceType, Type targetType)
            {
                this.sourceType = new SerializeType(sourceType);
                this.targetType = new SerializeType(targetType);
            }
        }
        [field: SerializeField]
        public Pair[] Pairs { get; set; }
        public Pair FindPair(NodeData.NodeType nodeType)
        {
            var serializeType = new SerializeType(nodeType);
            return Pairs.FirstOrDefault(x => x.sourceType.nodeType == serializeType.nodeType);
        }
#if UNITY_EDITOR
        internal static APIUpdateConfig GetConfig()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(APIUpdateConfig)}");
            if (guids.Length == 0)
            {
                return null;
            }
            return UnityEditor.AssetDatabase.LoadAssetAtPath<APIUpdateConfig>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
        }
#endif
    }
}
