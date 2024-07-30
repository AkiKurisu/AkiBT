namespace Kurisu.AkiBT.DSL
{
    /// <summary>
    /// Default compiler, used to demonstrate API combination and calling
    /// </summary>
    public class Compiler
    {
        private readonly NodeTypeRegistry registry;
        private bool verbose;
        public Compiler(NodeTypeRegistry registry)
        {
            this.registry = registry;
        }
        public Compiler(string registryPath)
        {
            registry = NodeTypeRegistry.FromPath(registryPath);
        }
        public BehaviorTree Compile(string code)
        {
            var lexer = new Lexer(new Reader(code), registry).Verbose(verbose);
            var bpl = BuildParserListener.GetPooled(BuildVisitor.GetPooled()).Verbose(verbose);
            try
            {
                lexer.ParseToEnd(new Parser(lexer, bpl));
                return bpl.Build();
            }
            finally
            {
                bpl.Dispose();
            }
        }
        public Compiler Verbose(bool verbose)
        {
            this.verbose = verbose;
            return this;
        }
    }
    public static class LexerExtensions
    {
        /// <summary>
        /// Parse all token until end from lexer
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="parser"></param>
        public static void ParseToEnd(this Lexer lexer, IParser parser)
        {
            lexer.GetNextToken();
            while (true)
            {
                switch (lexer.CurrentToken.Type)
                {
                    case TokenType.EOF:
                        return;
                    case TokenType.COMMENT:
                        DrainComment();
                        break;
                    case TokenType.DEF_VARIABLE:
                        parser.HandleVariableDefinition();
                        break;
                    default:
                        parser.HandleTopLevelExpression();
                        break;
                }
                lexer.GetNextToken();
            }
            void DrainComment()
            {
                lexer.GetNextToken();
                // Skip token within comment
                while (lexer.CurrentToken != TokenType.COMMENT)
                {
                    lexer.GetNextToken();
                }
            }
        }
    }
}
