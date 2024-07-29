namespace Kurisu.AkiBT.DSL
{
    public interface IParserListener
    {
        void PushVariableDefinition(VariableDefineExprAST data);
        void PushTopLevelExpression(NodeExprAST data);
    }
}