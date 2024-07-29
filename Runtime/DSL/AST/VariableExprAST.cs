namespace Kurisu.AkiBT.DSL
{
    // expr `secondVector3 => destination`
    /// <summary>
    /// Variable expression is a special case of property expression.
    /// </summary>
    public class VariableExprAST : PropertyExprAST
    {
        public VariableExprAST(PropertyInfo metaData, bool isShared, ValueExprAST exprAST) : base(metaData, exprAST)
        {
            ExprType = ExprType.VariableExpr;
            IsShared = isShared;
        }
        public bool IsShared { get; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitVariableExprAST(this);
        }
    }
}