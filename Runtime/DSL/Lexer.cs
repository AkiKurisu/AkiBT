using UnityEngine;
namespace Kurisu.AkiBT.DSL
{
    public sealed class Lexer
    {
        private readonly Reader reader;
        private readonly NodeTypeRegistry registry;
        private NodeInfo nodeInfo;
        private FieldType valueType;
        public Token CurrentToken { get; private set; }
        private ValueExprAST value;
        private bool verbose;
        public Lexer(Reader reader, NodeTypeRegistry nodeTypeRegistry)
        {
            this.reader = reader;
            registry = nodeTypeRegistry;
        }
        public Lexer Verbose(bool verbose)
        {
            this.verbose = verbose;
            return this;
        }
        public Token GetNextToken()
        {
            var word = reader.Read();
            if (word == null)
            {
                CurrentToken = new(TokenType.EOF);
            }
            else if (word == Symbol.Comment)
            {
                CurrentToken = new(TokenType.COMMENT);
            }
            else if (TryParseVariableType(word, out FieldType type))
            {
                valueType = type;
                CurrentToken = new(TokenType.DEF_VARIABLE, word);
            }
            else if (registry.TryGetNode(word, out NodeInfo info))
            {
                nodeInfo = info;
                CurrentToken = new(TokenType.DEF_NODE, word);
            }
            else if (TryParseValue(word))
            {
                valueType = value.Type;
                CurrentToken = new(TokenType.VALUE, value.Value.ToString());
            }
            else
            {
                CurrentToken = new Token(word);
            }
            if (verbose) Log(CurrentToken);
            return CurrentToken;
        }
        public ValueExprAST GetLastValue()
        {
            return value;
        }
        public NodeInfo GetLastNodeInfo()
        {
            return nodeInfo;
        }
        public FieldType GetLastValueType()
        {
            return valueType;
        }
        private bool TryParseVariableType(string token, out FieldType valueType)
        {
            valueType = default;
            if (token.Length < 3) return false;
            if (token[0] == '$' && token[^1] == '$')
            {
                return TryParseVariableType(token[1..^1], out valueType);
            }
            else
            {
                return TryParseVariableType(token, out valueType);
            }
            static bool TryParseVariableType(string token, out FieldType valueType)
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
                value = new(FieldType.Int, intNum);
                return true;
            }
            if (float.TryParse(token, out float floatNum))
            {
                value = new(FieldType.Float, floatNum);
                return true;
            }
            if (bool.TryParse(token, out bool boolValue))
            {
                value = new(FieldType.Bool, boolValue);
                return true;
            }
            // Aligned with reader index
            reader.MoveBack();
            if (TryParseVector2Int(out Vector2Int vector2Int))
            {
                value = new(FieldType.Vector2Int, vector2Int);
                return true;
            }
            if (TryParseVector2(out Vector2 vector2))
            {
                value = new(FieldType.Vector2, vector2);
                return true;
            }
            if (TryParseVector3Int(out Vector3Int vector3Int))
            {
                value = new(FieldType.Vector3Int, vector3Int);
                return true;
            }
            if (TryParseVector3(out Vector3 vector3))
            {
                value = new(FieldType.Vector3, vector3);
                return true;
            }
            // Missing string value?
            // Special case here since we can not know whether it is a symbol here
            reader.MoveNext();
            return false;
        }
        public bool TryParseVector2(out Vector2 vector2)
        {
            int bt = reader.CurrentIndex;
            try
            {
                vector2 = reader.ReadVector2();
                return true;
            }
            catch
            {
                reader.MoveTo(bt);
                vector2 = default;
                return false;
            }
        }
        public bool TryParseVector3(out Vector3 vector3)
        {
            int bt = reader.CurrentIndex;
            try
            {
                vector3 = reader.ReadVector3();
                return true;
            }
            catch
            {
                reader.MoveTo(bt);
                vector3 = default;
                return false;
            }
        }
        public bool TryParseVector3Int(out Vector3Int vector3Int)
        {
            int bt = reader.CurrentIndex;
            try
            {
                vector3Int = reader.ReadVector3Int();
                return true;
            }
            catch
            {
                reader.MoveTo(bt);
                vector3Int = default;
                return false;
            }
        }
        public bool TryParseVector2Int(out Vector2Int vector2Int)
        {
            int bt = reader.CurrentIndex;
            try
            {
                vector2Int = reader.ReadVector2Int();
                return true;
            }
            catch
            {
                reader.MoveTo(bt);
                vector2Int = default;
                return false;
            }
        }
        private static void Log(Token token)
        {
            Debug.Log($"<color=#33FF99>[Lexer] Read token: {token}</color>");
        }
    }
}