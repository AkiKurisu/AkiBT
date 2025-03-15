using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Kurisu.AkiBT.DSL
{
    /// <summary>
    /// Simple decompiler to convert any behavior tree to DSL
    /// </summary>
    public class Decompiler
    {
        private readonly StringBuilder _stringBuilder = new();
        
        public string Decompile(IBehaviorTreeContainer behaviorTreeContainer)
        {
            var instance = behaviorTreeContainer.GetBehaviorTree();
            _stringBuilder.Clear();
            WriteReferencedVariable(instance);
            WriteNode(instance.root.Child, 0);
            return _stringBuilder.ToString();
        }
        
        private void WriteReferencedVariable(BehaviorTree behaviorTreeContainer)
        {
            foreach (var variable in behaviorTreeContainer.SharedVariables)
            {
                if (variable.IsGlobal)
                    Write($"${variable.GetType().Name[6..]}$");
                else
                    Write(variable.GetType().Name[6..]);
                Space();
                Write(variable.Name);
                Space();
                var value = variable.GetValue();
                if (variable is SharedObject sharedObject && !string.IsNullOrEmpty(sharedObject.ConstraintTypeAQN))
                {
                    Type constraintType = Type.GetType(sharedObject.ConstraintTypeAQN);
                    SafeWrite($"{constraintType!.Assembly.GetName().Name}, {constraintType.Namespace}.{constraintType.Name}");
                    Space();
                }
#if UNITY_EDITOR
                if (value is UnityEngine.Object uObject)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(uObject));
                    if (string.IsNullOrEmpty(guid)) Write("Null");
                    else Write(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(uObject)));
                }
                else
#endif
                {
                    SafeWrite(value);
                }
                NewLine();
            }
        }
        
        private void WriteNode(NodeBehavior node, int indentLevel)
        {
            WriteIndent(indentLevel);
            var type = node.GetType();
            Write(type.Name);
            Write('(');
            WriteProperties(type, node, indentLevel);
            Write(')');
        }
        
        private void WriteProperties(Type type, NodeBehavior node, int indentLevel)
        {
            bool haveProperty = false;
            var properties = NodeTypeRegistry.GetAllFields(type).ToList();
            for (var i = 0; i < properties.Count; i++)
            {
                var p = properties[i];
                var value = p.GetValue(node);
                if (value == null) continue;
                if (haveProperty)
                {
                    Write(',');
                }
                else
                {
                    haveProperty = true;
                }
                WritePropertyName(p);
                var fieldType = p.FieldType;
                if (IsIList(fieldType, out Type elementType))
                {
                    Write(':');
                    WriteArray(value as IList, elementType, indentLevel);
                }
                else if (fieldType.IsSubclassOf(typeof(NodeBehavior)) || fieldType == typeof(NodeBehavior))
                {
                    Write(':');
                    NewLine();
                    WriteNode(value as NodeBehavior, indentLevel + 1);
                }
                else if (fieldType.IsSubclassOf(typeof(SharedVariable)) || fieldType == typeof(SharedVariable))
                {
                    WriteVariableValue(value as SharedVariable);
                }
                else
                {
                    WritePropertyValue(value);
                }
            }
        }
        
        private static bool IsIList(Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            elementType = null;
            return false;
        }
        
        private void WriteArray(IList list, Type childType, int indentLevel)
        {
            Write('[');
            for (var i = 0; i < list.Count; i++)
            {
                if (i != 0) Write(',');
                var value = list[i];
                if (childType.IsSubclassOf(typeof(NodeBehavior)) || childType == typeof(NodeBehavior))
                {
                    NewLine();
                    WriteNode(value as NodeBehavior, indentLevel + 1);
                }
                else if (childType.IsSubclassOf(typeof(SharedVariable)) || childType == typeof(SharedVariable))
                {
                    // TODO:
                }
                else
                {
                    SafeWrite(value);
                }
            }
            Write(']');
        }
        
        private void WritePropertyName(FieldInfo fieldInfo)
        {
            AkiLabelAttribute label;
            if ((label = fieldInfo.GetCustomAttribute<AkiLabelAttribute>()) != null)
            {
                Write(label.Title);
            }
            else
            {
                Write(fieldInfo.Name);
            }
        }
        
        private void WriteVariableValue(SharedVariable variable)
        {
            if (variable.IsShared)
            {
                Write("=>");
                Write(variable.Name);
            }
            else
            {
                Write(':');
                SafeWrite(variable.GetValue());
            }
        }
        
        private void WritePropertyValue(object value)
        {
            Write(':');
            SafeWrite(value);
        }
        
        private void Space()
        {
            _stringBuilder.Append(' ');
        }
        
        private void WriteIndent(int indentLevel)
        {
            _stringBuilder.Append(' ', indentLevel * 4);
        }
        
        private void NewLine()
        {
            _stringBuilder.Append('\n');
        }
        
        private void SafeWrite(object context)
        {
            if (context is string)
                _stringBuilder.Append($"\"{context}\"");
            else
                _stringBuilder.Append(context);
        }
        
        private void Write(string text)
        {
            _stringBuilder.Append(text);
        }
        
        private void Write(char token)
        {
            _stringBuilder.Append(token);
        }
    }
}
