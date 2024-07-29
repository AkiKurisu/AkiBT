namespace Kurisu.AkiBT.DSL
{
    public interface IParser
    {
        void HandleVariableDefinition();

        void HandleTopLevelExpression();
    }
}