using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kurisu.AkiBT.DSL
{
    public sealed class Lexer
    {
        private readonly Reader _reader;
        
        private readonly NodeTypeRegistry _registry;
        
        private NodeInfo _nodeInfo;
        
        private FieldType _valueType;
        
        public Token CurrentToken { get; private set; }
        
        private ValueExprAST _value;
        
        private bool _verbose;
        
        public Lexer(Reader reader, NodeTypeRegistry nodeTypeRegistry)
        {
            _reader = reader;
            _registry = nodeTypeRegistry;
        }
        
        public Lexer Verbose(bool verbose)
        {
            _verbose = verbose;
            return this;
        }
        
        public Token GetNextToken()
        {
            var word = _reader.Read();
            if (word == null)
            {
                CurrentToken = new Token(TokenType.EOF);
            }
            else if (word == Symbol.Comment)
            {
                CurrentToken = new Token(TokenType.COMMENT);
            }
            else if (TryParseVariableType(word, out FieldType type))
            {
                _valueType = type;
                CurrentToken = new Token(TokenType.DEF_VARIABLE, word);
            }
            else if (_registry.TryGetNode(word, out NodeInfo info))
            {
                _nodeInfo = info;
                CurrentToken = new Token(TokenType.DEF_NODE, word);
            }
            else if (TryParseValue(word))
            {
                _valueType = _value.Type;
                CurrentToken = new Token(TokenType.VALUE, _value.Value.ToString());
            }
            else
            {
                CurrentToken = new Token(word);
            }
            if (_verbose) Log(CurrentToken);
            return CurrentToken;
        }
        
        public ValueExprAST GetLastValue()
        {
            return _value;
        }
        
        public NodeInfo GetLastNodeInfo()
        {
            return _nodeInfo;
        }
        
        public FieldType GetLastValueType()
        {
            return _valueType;
        }
        
        private bool TryParseVariableType(string token, out FieldType valueType)
        {
            valueType = default;
            if (token.Length < 3) return false;
            if (token[0] == '$' && token[^1] == '$')
            {
                return TryParseVariableTypeLocal(token[1..^1], out valueType);
            }

            return TryParseVariableTypeLocal(token, out valueType);
            
            static bool TryParseVariableTypeLocal(string token, out FieldType valueType)
            {
                switch (token)
                {
                    case Symbol.Int:
                        {
                            valueType = FieldType.Int;
                            return true;
                        }
                    case Symbol.Bool:
                        {
                            valueType = FieldType.Bool;
                            return true;
                        }
                    case Symbol.Float:
                        {
                            valueType = FieldType.Float;
                            return true;
                        }
                    case Symbol.String:
                        {
                            valueType = FieldType.String;
                            return true;
                        }
                    case Symbol.Vector2:
                        {
                            valueType = FieldType.Vector2;
                            return true;
                        }
                    case Symbol.Vector2Int:
                        {
                            valueType = FieldType.Vector2Int;
                            return true;
                        }
                    case Symbol.Vector3:
                        {
                            valueType = FieldType.Vector3;
                            return true;
                        }
                    case Symbol.Vector3Int:
                        {
                            valueType = FieldType.Vector3Int;
                            return true;
                        }
                    case Symbol.Object:
                        {
                            valueType = FieldType.Object;
                            return true;
                        }
                }
                valueType = default;
                return false;
            }
        }
        
        private bool TryParseValue(string token)
        {
            if (int.TryParse(token, out int intNum))
            {
                _value = new ValueExprAST(FieldType.Int, intNum);
                return true;
            }
            if (float.TryParse(token, out float floatNum))
            {
                _value = new ValueExprAST(FieldType.Float, floatNum);
                return true;
            }
            if (bool.TryParse(token, out bool boolValue))
            {
                _value = new ValueExprAST(FieldType.Bool, boolValue);
                return true;
            }
            // Aligned with reader index
            _reader.MoveBack();
            if (TryParseVector2Int(out Vector2Int vector2Int))
            {
                _value = new ValueExprAST(FieldType.Vector2Int, vector2Int);
                return true;
            }
            if (TryParseVector2(out Vector2 vector2))
            {
                _value = new ValueExprAST(FieldType.Vector2, vector2);
                return true;
            }
            if (TryParseVector3Int(out Vector3Int vector3Int))
            {
                _value = new ValueExprAST(FieldType.Vector3Int, vector3Int);
                return true;
            }
            if (TryParseVector3(out Vector3 vector3))
            {
                _value = new ValueExprAST(FieldType.Vector3, vector3);
                return true;
            }
            // Missing string value?
            // Special case here since we can not know whether it is a symbol here
            _reader.MoveNext();
            return false;
        }
        
        public bool TryParseVector2(out Vector2 vector2)
        {
            int bt = _reader.CurrentIndex;
            try
            {
                vector2 = _reader.ReadVector2();
                return true;
            }
            catch
            {
                _reader.MoveTo(bt);
                vector2 = default;
                return false;
            }
        }
        
        public bool TryParseVector3(out Vector3 vector3)
        {
            int bt = _reader.CurrentIndex;
            try
            {
                vector3 = _reader.ReadVector3();
                return true;
            }
            catch
            {
                _reader.MoveTo(bt);
                vector3 = default;
                return false;
            }
        }
        
        public bool TryParseVector3Int(out Vector3Int vector3Int)
        {
            int bt = _reader.CurrentIndex;
            try
            {
                vector3Int = _reader.ReadVector3Int();
                return true;
            }
            catch
            {
                _reader.MoveTo(bt);
                vector3Int = default;
                return false;
            }
        }
        
        public bool TryParseVector2Int(out Vector2Int vector2Int)
        {
            int bt = _reader.CurrentIndex;
            try
            {
                vector2Int = _reader.ReadVector2Int();
                return true;
            }
            catch
            {
                _reader.MoveTo(bt);
                vector2Int = default;
                return false;
            }
        }
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log(Token token)
        {
            Debug.Log($"<color=#33FF99>[Lexer] Read token: {token}</color>");
        }
    }
}