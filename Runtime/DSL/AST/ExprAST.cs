namespace Kurisu.AkiBT.DSL
{
    public enum ExprType
    {
        NodeExpr,
        PropertyExpr,
        ArrayExpr,
        VariableDefineExpr,
        ObjectDefineExpr,
        VariableExpr,
        ValueExpr
    }
    public abstract class ExprAST
    {
        public abstract ExprType ExprType { get; protected set; }
        protected internal virtual ExprAST VisitChildren(ExprVisitor visitor)
        {
            return visitor.Visit(this);
        }

        protected internal virtual ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitExtension(this);
        }
    }
}
