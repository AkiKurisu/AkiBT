using System;
using System.Linq;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [CreateAssetMenu(fileName = "APIUpdateConfig", menuName = "AkiBT/APIUpdateConfig")]
    public class APIUpdateConfig : ScriptableObject
    {
        [Serializable]
        internal class Redirector<T>
        {
            public T source;
            
            public T target;
        }
        
        [Serializable]
        public class SerializeType
        {
            private Type _type;
            
            public Type Type => _type ??= ToType();
            
            public string nodeType;
            
            public Type ToType()
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
        

        [SerializeField]
        internal Redirector<SerializeType>[] nodeRedirectors;
        
        [SerializeField]
        private Redirector<string>[] assemblyRedirectors;
        
        [SerializeField]
        private Redirector<string>[] namespaceRedirectors;
        
        public Type RedirectNode(in NodeData.NodeType nodeType)
        {
            var serializeType = new SerializeType(nodeType);
            var redirector = nodeRedirectors.FirstOrDefault(x => x.source == serializeType);
            return redirector?.target.ToType() ?? RedirectAssemblyAndNamespace(nodeType);
        }
        
        private Type RedirectAssemblyAndNamespace(NodeData.NodeType nodeType)
        {
            NodeData.NodeType redirectedNodeType = nodeType;
            var assemblyRedirector = assemblyRedirectors.FirstOrDefault(r => nodeType._asm.StartsWith(r.source));
            if (assemblyRedirector != null)
            {
                redirectedNodeType._asm = redirectedNodeType._asm.Replace(assemblyRedirector.source, assemblyRedirector.target);
            }
            
            var namespaceRedirector = namespaceRedirectors.FirstOrDefault(r => nodeType._ns.StartsWith(r.source));
            if (namespaceRedirector != null)
            {
                redirectedNodeType._ns = redirectedNodeType._ns.Replace(namespaceRedirector.source, namespaceRedirector.target);
            }

            if (redirectedNodeType.Equals(nodeType))
            {
                return null;
            }

            return redirectedNodeType.ToType();
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
