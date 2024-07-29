using System;
namespace Kurisu.AkiBT.DSL
{
    public enum TokenType
    {
        // Dynamic token, need be interpreted by parser
        DYNAMIC,
        EOF,
        // Code comment
        COMMENT,
        // Define sharedVariable
        DEF_VARIABLE,
        // Define node
        DEF_NODE,
        // Value type
        VALUE
    }
    /// <summary>
    /// Each token returned by the lexer includes a token type and potentially a string value
    /// </summary>
    public readonly struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public Token(TokenType tokenType)
        {
            Type = tokenType;
            Value = default;
        }
        public Token(string value)
        {
            Type = TokenType.DYNAMIC;
            Value = value;
        }
        public Token(TokenType tokenType, string value)
        {
            Type = tokenType;
            Value = value;
        }
        public static bool operator ==(Token first, Token second)
        {
            return first.Type == second.Type && first.Value == second.Value;
        }

        public static bool operator !=(Token first, Token second)
        {
            return first.Type != second.Type || first.Value != second.Value;
        }
        public static bool operator ==(Token first, TokenType second)
        {
            return first.Type == second;
        }

        public static bool operator !=(Token first, TokenType second)
        {
            return first.Type != second;
        }
        public static bool operator ==(Token first, string second)
        {
            return first.Value == second;
        }

        public static bool operator !=(Token first, string second)
        {
            return first.Value != second;
        }
        public bool Equals(Token other)
        {
            return other.Type == Type && other.Value == Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Token token && Equals(token);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }

        public override string ToString()
        {
            return $"[{Type}]{Value}";
        }
    }
}
