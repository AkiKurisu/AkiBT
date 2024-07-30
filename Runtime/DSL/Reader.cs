using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
namespace Kurisu.AkiBT.DSL
{
    public sealed class Reader
    {
        private const string Pattern = @"(\(|\)|\[|\,|\:|\]| |\n|\r|=>|\t)";
        private static readonly string[] ignoreTokens = new[] { "\n", "\r", "\t", " ", "" };
        private readonly string[] tokens;
        public string CurrentToken
        {
            get
            {
                if (CurrentIndex < tokens.Length) return tokens[CurrentIndex];
                return null;
            }
        }
        public int CurrentIndex { get; private set; } = 0;
        public Reader(string[] tokens)
        {
            this.tokens = tokens;
        }
        public Reader(string stream)
        {
            tokens = Tokenize(stream);
        }
        /// <summary>
        /// Read token, if not exist return null 
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            int index = CurrentIndex++;
            if (index < tokens.Length)
                return tokens[index];
            return null;
        }
        public void MoveBack()
        {
            CurrentIndex--;
        }
        public void MoveNext()
        {
            CurrentIndex++;
        }
        public void MoveTo(int index)
        {
            CurrentIndex = index;
        }
        private static string[] Tokenize(string code)
        {
            int start = 0;
            int flag = 0;
            var tokens = new List<string>();
            for (int i = 0; i < code.Length; ++i)
            {
                if (code[i] == '\"')
                {
                    if (flag == 0)
                    {
                        tokens.AddRange(Regex.Split(code[start..i], Pattern));
                        start = i + 1;
                        flag = 1;
                    }
                    else
                    {
                        tokens.Add(code[start..i]);
                        start = i + 1;
                        flag = 0;
                    }
                }
            }
            if (start < code.Length)
                tokens.AddRange(Regex.Split(code[(start - flag)..], Pattern));
            return tokens.Where(x => !ignoreTokens.Contains(x)).ToArray();
        }
        public Vector3 ReadVector3()
        {
            float x, y, z;
            AssertToken(Read(), Symbol.LeftParenthesis);
            x = float.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            y = float.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            z = float.Parse(Read());
            AssertToken(Read(), Symbol.RightParenthesis);
            return new Vector3(x, y, z);
        }
        public Vector3Int ReadVector3Int()
        {
            int x, y, z;
            AssertToken(Read(), Symbol.LeftParenthesis);
            x = int.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            y = int.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            z = int.Parse(Read());
            AssertToken(Read(), Symbol.RightParenthesis);
            return new Vector3Int(x, y, z);
        }
        public Vector2 ReadVector2()
        {
            float x, y;
            AssertToken(Read(), Symbol.LeftParenthesis);
            x = float.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            y = float.Parse(Read());
            AssertToken(Read(), Symbol.RightParenthesis);
            return new Vector2(x, y);
        }
        public Vector2Int ReadVector2Int()
        {
            int x, y;
            AssertToken(Read(), Symbol.LeftParenthesis);
            x = int.Parse(Read());
            AssertToken(Read(), Symbol.Comma);
            y = int.Parse(Read());
            AssertToken(Read(), Symbol.RightParenthesis);
            return new Vector2Int(x, y);
        }
        private static void AssertToken(string token, string assertToken)
        {
            Assert.IsTrue(token == assertToken);
        }
    }
}