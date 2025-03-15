using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using Debug = UnityEngine.Debug;
using UObject = UnityEngine.Object;
namespace Kurisu.AkiBT.DSL
{
    /// <summary>
    /// Runtime supported expression visitor to build behavior tree
    /// </summary>
    public class BuildVisitor : ExprVisitor, IDisposable
    {
        private bool _isPooled;
        
        private static readonly ObjectPool<BuildVisitor> Pool = new(() => new BuildVisitor());
        
        public bool Verbose { get; set; }
        
        public readonly Stack<SharedVariable> VariableStack = new();
        
        public readonly Stack<NodeBehavior> NodeStack = new();
        
        public readonly Stack<object> ValueStack = new();
        
        protected internal override ExprAST VisitNodeExprAST(NodeExprAST node)
        {
            var nodeInfo = node.MetaData;
            var instance = Activator.CreateInstance(nodeInfo.GetNodeType()) as NodeBehavior;
            // Push if root
            if (NodeStack.Count == 0)
                NodeStack.Push(instance);
            // Push to node stack for writing properties
            NodeStack.Push(instance);
            base.VisitNodeExprAST(node);
            // Push to value stack for using as value expr
            ValueStack.Push(instance);
            NodeStack.Pop();
            if (Verbose) Log($"Succeed build node {nodeInfo.GetNodeType().Name}");
            return node;
        }
        
        protected internal override ExprAST VisitPropertyAST(PropertyExprAST node)
        {
            NodeBehavior parent = NodeStack.Peek();
            base.VisitPropertyAST(node);
            FieldInfo fieldInfo = node.MetaData.FieldInfo;
            // Pop value from stack
            var boxValue = ValueStack.Pop();
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
            ValueStack.Push(node.Value);
            return node;
        }
        
        protected internal override ExprAST VisitArrayExprAST(ArrayExprAST node)
        {
            IList array = (IList)Activator.CreateInstance(node.FieldType, node.Children.Count);
            if (array.IsFixedSize)
            {
                int i = 0;
                foreach (var child in node.Children)
                {
                    Visit(child);
                    array[i++] = ValueStack.Pop();
                }
            }
            else
            {
                foreach (var child in node.Children)
                {
                    Visit(child);
                    array.Add(ValueStack.Pop());
                }
            }
            ValueStack.Push(array);
            return node;
        }
        
        protected internal override ExprAST VisitVariableDefineAST(VariableDefineExprAST node)
        {
            base.VisitVariableDefineAST(node);
            SharedVariable variable = CreateInstance(node.Type);
            variable.Name = node.Name;
            variable.IsGlobal = node.IsGlobal;
            variable.IsExposed = node.IsGlobal;
            var value = ValueStack.Pop();
            if (!NodeTypeRegistry.IsFieldType(value, node.Type))
            {
                value = NodeTypeRegistry.Cast(in value, value.GetType(), NodeTypeRegistry.GetValueType(node.Type));
            }
            variable.SetValue(value);
            if (Verbose) Log($"Build variable {variable.Name}, type: {node.Type}, value: {variable.GetValue()}");
            VariableStack.Push(variable);
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
            WriteSharedObjectValue(variable, ValueStack.Pop() as string);
            if (Verbose) Log($"Build variable {variable.Name}, type: {node.Type}, value: {variable.GetValue()}");
            VariableStack.Push(variable);
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
            NodeBehavior parent = NodeStack.Peek();
            base.VisitPropertyAST(node);
            FieldInfo fieldInfo = node.MetaData.FieldInfo;
            // This will also create instance for SharedTObject<UObject>
            SharedVariable variable = (SharedVariable)Activator.CreateInstance(fieldInfo.FieldType);
            if (node.IsShared)
            {
                variable.Name = ValueStack.Pop() as string;
                variable.IsShared = true;
            }
            else
            {
                // SharedTObject<UObject> or SharedObject
                if (variable is IBindableVariable<UObject> sharedObject)
                {
                    WriteSharedObjectValue(sharedObject, ValueStack.Pop() as string);
                }
                else
                {
                    variable.SetValue(ValueStack.Pop());
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
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log(string message)
        {
            Debug.Log($"<color=#9999FF>[Build Visitor] {message}</color>");
        }
        
        public static BuildVisitor GetPooled()
        {
            var visitor = Pool.Get();
            visitor._isPooled = true;
            return visitor;
        }
        
        public void Dispose()
        {
            VariableStack.Clear();
            ValueStack.Clear();
            NodeStack.Clear();
            // Only release visitor from pool
            if (_isPooled)
            {
                _isPooled = false;
                Pool.Release(this);
            }
        }
    }
}