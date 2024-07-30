using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;
using UObject = UnityEngine.Object;
namespace Kurisu.AkiBT.DSL
{
    /// <summary>
    /// Runtime supported expression visitor to build behavior tree
    /// </summary>
    public class BuildVisitor : ExprVisitor, IDisposable
    {
        private bool isPooled;
        private static readonly ObjectPool<BuildVisitor> pool = new(() => new());
        public bool Verbose { get; set; }
        public readonly Stack<SharedVariable> variableStack = new();
        public readonly Stack<NodeBehavior> nodeStack = new();
        public readonly Stack<object> valueStack = new();
        protected internal override ExprAST VisitNodeExprAST(NodeExprAST node)
        {
            var nodeInfo = node.MetaData;
            var instance = Activator.CreateInstance(nodeInfo.GetNodeType()) as NodeBehavior;
            // Push if root
            if (nodeStack.Count == 0)
                nodeStack.Push(instance);
            // Push to node stack for writing properties
            nodeStack.Push(instance);
            base.VisitNodeExprAST(node);
            // Push to value stack for using as value expr
            valueStack.Push(instance);
            nodeStack.Pop();
            if (Verbose) Log($"Succeed build node {nodeInfo.GetNodeType().Name}");
            return node;
        }
        protected internal override ExprAST VisitPropertyAST(PropertyExprAST node)
        {
            NodeBehavior parent = nodeStack.Peek();
            base.VisitPropertyAST(node);
            FieldInfo fieldInfo = node.MetaData.FieldInfo;
            // Pop value from stack
            var boxValue = valueStack.Pop();
            Type boxType = boxValue.GetType();
            if (boxType != fieldInfo.FieldType)
            {
                boxValue = NodeTypeRegistry.Cast(in boxValue, boxType, fieldInfo.FieldType);
            }
            fieldInfo.SetValue(parent, boxValue);
            if (Verbose) Log($"Write property {fieldInfo.Name} to {parent}");
            return node;
        }
        protected internal override ExprAST VisitValueExprAST(ValueExprAST node)
        {
            base.VisitValueExprAST(node);
            // Push value to task
            valueStack.Push(node.Value);
            return node;
        }
        protected internal override ExprAST VisitArrayExprAST(ArrayExprAST node)
        {
            IList array = Activator.CreateInstance(node.FieldType, node.Children.Count) as IList;
            if (array.IsFixedSize)
            {
                int i = 0;
                foreach (var child in node.Children)
                {
                    Visit(child);
                    array[i++] = valueStack.Pop();
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    Visit(child);
                    array.Add(valueStack.Pop());
                }
            }
            valueStack.Push(array);
            return node;
        }
        protected internal override ExprAST VisitVariableDefineAST(VariableDefineExprAST node)
        {
            base.VisitVariableDefineAST(node);
            SharedVariable variable = CreateInstance(node.Type);
            variable.Name = node.Name;
            variable.IsGlobal = node.IsGlobal;
            variable.IsExposed = node.IsGlobal;
            var value = valueStack.Pop();
            if (!NodeTypeRegistry.IsFieldType(value, node.Type))
            {
                value = NodeTypeRegistry.Cast(in value, value.GetType(), NodeTypeRegistry.GetValueType(node.Type));
            }
            variable.SetValue(value);
            if (Verbose) Log($"Build variable {variable.Name}, type: {node.Type}, value: {variable.GetValue()}");
            variableStack.Push(variable);
            return node;
        }
        protected internal override ExprAST VisitObjectDefineAST(ObjectDefineExprAST node)
        {
            base.VisitObjectDefineAST(node);
            SharedObject variable = CreateInstance(node.Type) as SharedObject;
            variable.Name = node.Name;
            variable.IsGlobal = node.IsGlobal;
            variable.IsExposed = node.IsGlobal;
            variable.ConstraintTypeAQN = node.ConstraintTypeAQN;
            WriteSharedObjectValue(variable, valueStack.Pop() as string);
            if (Verbose) Log($"Build variable {variable.Name}, type: {node.Type}, value: {variable.GetValue()}");
            variableStack.Push(variable);
            return node;
        }
        /// <summary>
        /// Override to implement UnityEngine.Object parse logic at runtime, such as loading by address
        /// </summary>
        /// <param name="sharedObject"></param>
        /// <param name="value"></param>
        protected virtual void WriteSharedObjectValue(IBindableVariable<UObject> sharedObject, string value)
        {
            // Default parse as guid in Editor
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(value);
            if (!string.IsNullOrEmpty(path))
            {
                sharedObject.Value = UnityEditor.AssetDatabase.LoadAssetAtPath<UObject>(path);
            }
            else
#endif
            {
                sharedObject.Value = null;
            }
        }
        protected internal override ExprAST VisitVariableExprAST(VariableExprAST node)
        {
            NodeBehavior parent = nodeStack.Peek();
            base.VisitPropertyAST(node);
            FieldInfo fieldInfo = node.MetaData.FieldInfo;
            // This will also create instance for SharedTObject<UObject>
            SharedVariable variable = Activator.CreateInstance(fieldInfo.FieldType) as SharedVariable;
            if (node.IsShared)
            {
                variable.Name = valueStack.Pop() as string;
                variable.IsShared = true;
            }
            else
            {
                // SharedTObject<UObject> or SharedObject
                if (variable is IBindableVariable<UObject> sharedObject)
                {
                    WriteSharedObjectValue(sharedObject, valueStack.Pop() as string);
                }
                else
                {
                    variable.SetValue(valueStack.Pop());
                }
            }
            fieldInfo.SetValue(parent, variable);
            return node;
        }
        /// <summary>
        /// Create a <see cref="SharedVariable"/> instance by field type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SharedVariable CreateInstance(FieldType type)
        {
            return type switch
            {
                FieldType.Int => new SharedInt(),
                FieldType.Float => new SharedFloat(),
                FieldType.Bool => new SharedBool(),
                FieldType.String => new SharedString(),
                FieldType.Vector2 => new SharedVector2(),
                FieldType.Vector3 => new SharedVector3(),
                FieldType.Vector2Int => new SharedVector2Int(),
                FieldType.Vector3Int => new SharedVector3Int(),
                FieldType.Object => new SharedObject(),
                _ => throw new ArgumentException(nameof(type)),
            };
        }
        protected static void Log(string message)
        {
            Debug.Log($"<color=#9999FF>[Build Visitor] {message}</color>");
        }
        public static BuildVisitor GetPooled()
        {
            var visitor = pool.Get();
            visitor.isPooled = true;
            return visitor;
        }
        public void Dispose()
        {
            variableStack.Clear();
            valueStack.Clear();
            nodeStack.Clear();
            // Only release visitor from pool
            if (isPooled)
            {
                isPooled = false;
                pool.Release(this);
            }
        }
    }
}