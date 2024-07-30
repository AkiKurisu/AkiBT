using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UObject = UnityEngine.Object;
using UnityEngine.Assertions;
namespace Kurisu.AkiBT.DSL
{
    public class NodeTypeRegistry
    {
        [JsonProperty]
        private Dictionary<string, NodeInfo> NodeInfos { get; set; } = new();
        public static readonly HashSet<ITypeContract> contracts = new();
        static NodeTypeRegistry()
        {
            contracts.Add(new Vector2IntToVector2Contract());
            contracts.Add(new Vector3IntToVector3Contract());
            contracts.Add(new Vector2ToVector3Contract());
            contracts.Add(new Vector3ToVector2Contract());
        }
        public static NodeTypeRegistry FromPath(string path)
        {
            return JsonConvert.DeserializeObject<NodeTypeRegistry>(File.ReadAllText(path));
        }
        /// <summary>
        /// Get node meta data from node path
        /// </summary>
        /// <param name="nodePath"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public bool TryGetNode(string nodePath, out NodeInfo metaData)
        {
            if (NodeInfos.TryGetValue(nodePath, out metaData))
            {
                // Lazy call to cache all fields
                metaData.GetNodeType();
                return !metaData.isVariable;
            }
            return false;
        }
        /// <summary>
        /// Register a new node
        /// </summary>
        /// <param name="nodePath"></param>
        /// <param name="metaData"></param>
        public void SetNode(string nodePath, NodeInfo metaData)
        {
            NodeInfos[nodePath] = metaData;
        }
        public static object Cast(in object value, Type inputType, Type expectType)
        {
            foreach (var contract in contracts)
            {
                if (contract.CanConvert(inputType, expectType))
                {
                    return contract.Convert(value, inputType, expectType);
                }
            }
            return value;
        }
        /// <summary>
        /// Get <see cref="FieldType"/> from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FieldType GetFieldType(Type type)
        {
            if (type.IsSubclassOf(typeof(SharedVariable))) return FieldType.Variable;
            if (type.IsEnum) return FieldType.Enum;
            if (type == typeof(int)) return FieldType.Int;
            if (type == typeof(float)) return FieldType.Float;
            if (type == typeof(bool)) return FieldType.Bool;
            if (type == typeof(string)) return FieldType.String;
            if (type == typeof(Vector2)) return FieldType.Vector2;
            if (type == typeof(Vector2Int)) return FieldType.Vector2Int;
            if (type == typeof(Vector3)) return FieldType.Vector3;
            if (type == typeof(Vector3Int)) return FieldType.Vector3Int;
            if (type.IsSubclassOf(typeof(UObject))) return FieldType.Object;
            return FieldType.Unknown;
        }
        /// <summary>
        /// Get value type from <see cref="FieldType"/>
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static Type GetValueType(FieldType fieldType)
        {
            Assert.IsTrue((int)fieldType <= 7);
            return fieldType switch
            {
                FieldType.Int => typeof(int),
                FieldType.Float => typeof(float),
                FieldType.Bool => typeof(bool),
                FieldType.Vector2 => typeof(Vector2),
                FieldType.Vector2Int => typeof(Vector2Int),
                FieldType.Vector3 => typeof(Vector3),
                FieldType.Vector3Int => typeof(Vector3Int),
                FieldType.String => typeof(string),
                _ => throw new ArgumentOutOfRangeException(nameof(fieldType)),
            };
        }
        public static bool IsFieldType(object value, FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Variable => value is SharedObject,
                FieldType.Enum => value is Enum,
                FieldType.Int => value is int,
                FieldType.Float => value is float,
                FieldType.Bool => value is bool,
                FieldType.String => value is string,
                FieldType.Vector2 => value is Vector2,
                FieldType.Vector2Int => value is Vector2Int,
                FieldType.Vector3 => value is Vector3,
                FieldType.Vector3Int => value is Vector3Int,
                FieldType.Object => value is UObject,
                FieldType.Unknown => false,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldType)),
            };
        }
        public static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Concat(GetAllFields_Internal(type))
                    .Where(field => field.IsInitOnly == false && field.GetCustomAttribute<HideInEditorWindow>() == null)
                    .ToList();
        }
        private static IEnumerable<FieldInfo> GetAllFields_Internal(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttribute<SerializeField>() != null || field.GetCustomAttribute<SerializeReference>() != null)
                    .Concat(GetAllFields_Internal(t.BaseType));
        }
    }
    public class NodeInfo
    {
        public string className;
        public string ns;
        public string asm;
        public bool isVariable;
        public List<PropertyInfo> properties;
        private List<FieldInfo> fieldInfos;
        private Type type;
        public PropertyInfo GetProperty(string label)
        {
            return properties?.FirstOrDefault(x => x.label == label || x.name == label);
        }
        public Type GetNodeType()
        {
            type ??= Type.GetType(Assembly.CreateQualifiedName(asm, $"{ns}.{className}"));
            if (fieldInfos == null)
            {
                fieldInfos = NodeTypeRegistry.GetAllFields(type).ToList();
                foreach (var property in properties)
                {
                    var field = fieldInfos.FirstOrDefault(x => x.Name == property.name);
                    if (field != null)
                        property.FieldInfo = field;
                }
            }
            return type;
        }
    }
    public class PropertyInfo
    {
        public string label;
        public string name;
        public FieldType fieldType;
        [JsonIgnore]
        public bool IsVariable => fieldType == FieldType.Variable;
        [JsonIgnore]
        public bool IsEnum => fieldType == FieldType.Enum;
        [JsonIgnore]
        public FieldInfo FieldInfo { get; internal set; }
    }
}
