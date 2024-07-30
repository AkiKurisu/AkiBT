using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
namespace Kurisu.AkiBT.DSL
{
    public sealed class Parser : IParser
    {
        private const string FullAQNPattern = @"^[A-Za-z0-9\.]+,\s?[A-Za-z0-9\.]+,\s?Version=\d+\.\d+\.\d+\.\d+,\s?Culture=[a-zA-Z]+,\s?PublicKeyToken=[a-zA-Z0-9]+";
        private const string HalfAQNPattern = @"^[A-Za-z0-9\.]+,\s?[A-Za-z0-9\.]+";
        private readonly Lexer lexer;
        private readonly IParserListener parserListener;
        public Parser(Lexer lexer, IParserListener parserListener)
        {
            this.lexer = lexer;
            this.parserListener = parserListener;
        }
        public void HandleVariableDefinition()
        {
            var variableAST = ParseVariableDefine();
            if (variableAST != null)
            {
                parserListener.PushVariableDefinition(variableAST);
            }
            else
            {
                // Skip token for error recovery.
                lexer.GetNextToken();
            }
        }
        public void HandleTopLevelExpression()
        {
            var nodeAST = ParseNode();
            if (nodeAST != null)
            {
                parserListener.PushTopLevelExpression(nodeAST);
            }
            else
            {
                // Skip token for error recovery.
                lexer.GetNextToken();
            }
        }
        private NodeExprAST ParseNode()
        {
            if (lexer.CurrentToken != TokenType.DEF_NODE)
            {
                LogError("Expected define node type");
                return null;
            }
            // node info is cached in scanner
            var metaData = lexer.GetLastNodeInfo();
            if (lexer.GetNextToken() != Symbol.LeftParenthesis)
            {
                LogError("Expected '(' in prototype");
                return null;
            }
            var properties = new List<PropertyExprAST>();
            if (lexer.GetNextToken() != Symbol.RightParenthesis)
            {
                while (true)
                {
                    PropertyExprAST property = ParseProperty(metaData);
                    if (property == null)
                    {
                        return null;
                    }
                    properties.Add(property);
                    if (lexer.CurrentToken == Symbol.RightParenthesis)
                    {
                        break;
                    }
                    if (lexer.CurrentToken != Symbol.Comma)
                    {
                        LogError("Expected ')' or ',' in property list");
                        return null;
                    }
                    lexer.GetNextToken();
                }
            }
            // eat ')'
            lexer.GetNextToken();
            return new NodeExprAST(metaData, properties);
        }
        private PropertyExprAST ParseProperty(NodeInfo nodeInfo)
        {
            Assert.IsNotNull(nodeInfo);
            if (lexer.CurrentToken != TokenType.DYNAMIC)
            {
                LogError("Expected dynamic token");
                return null;
            }
            // property need register in NodeTypeRegistry to identify right value type
            var propertyInfo = nodeInfo.GetProperty(lexer.CurrentToken.Value);
            if (propertyInfo == null)
            {
                LogError("Expected valid property");
                return null;
            }
            var fieldInfo = propertyInfo.FieldInfo;
            if (fieldInfo == null)
            {
                LogError($"Field {propertyInfo.name} not exist in {nodeInfo.GetNodeType()}");
                return null;
            }
            Type fieldType = fieldInfo.FieldType;
            lexer.GetNextToken();
            // Thanks to meta data, easy to find out here
            if (propertyInfo.IsVariable)
            {
                return ParseVariable();
            }
            // Special case for enum
            else if (propertyInfo.IsEnum)
            {
                return ParseEnum();
            }
            else
            {
                return ParseExpr();
            }
            PropertyExprAST ParseExpr()
            {
                if (lexer.CurrentToken != Symbol.Colon)
                {
                    LogError($"Expected {Symbol.Colon} after define property name");
                    return null;
                }
                lexer.GetNextToken();
                ExprAST expr = ParseExpression(fieldType);
                if (expr == null)
                {
                    LogError("Expected property value");
                    return null;
                }
                return new PropertyExprAST(propertyInfo, expr);
            }
            // Handle special case for enum
            PropertyExprAST ParseEnum()
            {
                if (lexer.CurrentToken != Symbol.Colon)
                {
                    LogError($"Expected {Symbol.Colon} after define property name");
                    return null;
                }
                lexer.GetNextToken();
                ValueExprAST expr;
                if (lexer.CurrentToken == TokenType.VALUE && lexer.GetLastValueType() == FieldType.Int)
                {
                    expr = new(FieldType.Enum, lexer.GetLastValue().Value);
                }
                else
                {
                    if (Enum.TryParse(fieldType, lexer.CurrentToken.Value, out object enumValue))
                    {
                        expr = new(FieldType.Enum, enumValue);
                    }
                    else
                    {
                        LogError($"Can not parse enum {fieldType} from {lexer.CurrentToken.Value}");
                        return null;
                    }
                }
                //eat ')' or ','
                lexer.GetNextToken();
                return new PropertyExprAST(propertyInfo, expr);
            }
            // Handle special case for variable
            VariableExprAST ParseVariable()
            {
                bool isShared;
                if (lexer.CurrentToken == Symbol.Shared)
                {
                    isShared = true;
                }
                else
                {
                    isShared = false;
                    if (lexer.CurrentToken != Symbol.Colon)
                    {
                        LogError($"Expected {Symbol.Colon} after define property name");
                        return null;
                    }
                }
                lexer.GetNextToken();
                ValueExprAST value;
                if (lexer.CurrentToken == TokenType.VALUE)
                {
                    value = ParseValue();
                    if (value == null)
                    {
                        LogError("Expected value");
                        return null;
                    }
                    // TODO: Not type safe
                    return new VariableExprAST(propertyInfo, isShared, value);
                }
                else
                {
                    // Identify reference type [String|UObject] as string => can not identify UObject in parser which missing meta data
                    value = ValueExprAST.String(lexer.CurrentToken.Value);
                    //eat ')' or ','
                    lexer.GetNextToken();
                    return new VariableExprAST(propertyInfo, isShared, value);
                }
            }
        }
        private ExprAST ParseExpression(Type fieldType)
        {
            if (lexer.CurrentToken == Symbol.LeftBracket)
            {
                var array = ParseArray(fieldType);
                if (array == null)
                {
                    LogError("Expected array");
                    return null;
                }
                return array;
            }
            // Reference value
            else if (lexer.CurrentToken == TokenType.DEF_NODE)
            {
                var node = ParseNode();
                if (node == null)
                {
                    LogError("Expected node");
                    return null;
                }
                return node;
            }
            // Simple value
            else if (lexer.CurrentToken == TokenType.VALUE)
            {
                var valueExp = ParseValue();
                if (valueExp == null)
                {
                    LogError("Expected value");
                    return null;
                }
                // TODO: Not type safe
                return valueExp;
            }
            // Identify reference type [String|UObject] as string => can not identify UObject in parser which missing meta data
            return ValueExprAST.String(lexer.CurrentToken.Value);
        }
        private ValueExprAST ParseValue()
        {
            if (lexer.CurrentToken != TokenType.VALUE)
            {
                LogError("Expected value token");
                return null;
            }
            var valueExp = lexer.GetLastValue();
            if (valueExp == null)
            {
                LogError("Expected value");
                return null;
            }
            lexer.GetNextToken();
            return valueExp;
        }
        private ArrayExprAST ParseArray(Type fieldType)
        {
            if (lexer.CurrentToken != Symbol.LeftBracket)
            {
                LogError($"Expected {Symbol.LeftBracket} before array start");
                return null;
            }
            var values = new List<ExprAST>();
            if (lexer.GetNextToken() != Symbol.RightBracket)
            {
                while (true)
                {
                    // Assert containing generic argument
                    ExprAST value = ParseExpression(fieldType.GetGenericArguments()[0]);
                    if (value == null)
                    {
                        return null;
                    }
                    values.Add(value);
                    if (lexer.CurrentToken == Symbol.RightBracket)
                    {
                        break;
                    }
                    if (lexer.CurrentToken != Symbol.Comma)
                    {
                        LogError("Expected ']' or ',' in array");
                        return null;
                    }
                    lexer.GetNextToken();
                }
            }
            // eat `]`
            lexer.GetNextToken();
            return new ArrayExprAST(fieldType, values);
        }
        private VariableDefineExprAST ParseVariableDefine()
        {
            if (lexer.CurrentToken != TokenType.DEF_VARIABLE)
            {
                LogError("Expected define variable type");
                return null;
            }
            var word = lexer.CurrentToken.Value;
            bool isGlobal = word[0] == '$' && word[^1] == '$';
            FieldType valueType = lexer.GetLastValueType();
            string name = lexer.GetNextToken().Value;
            var token = lexer.GetNextToken();
            if (token.Type == TokenType.VALUE)
            {
                if (lexer.GetLastValueType() != valueType)
                    LogWarning($"Expected value type {valueType.ToString().ToLower()}, but get {lexer.GetLastValueType().ToString().ToLower()}");
                // May can be convert, so not return null
                return new VariableDefineExprAST(valueType, isGlobal, name, lexer.GetLastValue());
            }
            if (valueType == FieldType.Object)
            {
                string aqn = null;
                string stringValue = token.Value;
                // Allow two kinds of constraint define format
                if (Regex.IsMatch(stringValue, FullAQNPattern))
                {
                    aqn = stringValue;
                }
                // Half and reverse
                else if (Regex.IsMatch(stringValue, HalfAQNPattern))
                {
                    var items = stringValue.Split(',');
                    aqn = Assembly.CreateQualifiedName(items[0].Trim(), items[1].Trim());
                }
                if (string.IsNullOrEmpty(aqn))
                    //No aqn define
                    return new ObjectDefineExprAST(valueType, isGlobal, name, null, ValueExprAST.String(stringValue));
                //Move next to get actual value
                return new ObjectDefineExprAST(valueType, isGlobal, name, aqn, ValueExprAST.String(lexer.GetNextToken().Value));
            }
            //Identify as string, need visitor to read value.
            return new VariableDefineExprAST(valueType, isGlobal, name, ValueExprAST.String(token.Value));
        }
        private static void LogWarning(object message)
        {
            Debug.LogWarning($"<color=#FFFF66>[Parser] {message}</color>");
        }
        private static void LogError(object message)
        {
            Debug.LogError($"<color=#FF6666>[Parser] {message}</color>");
        }
    }
}