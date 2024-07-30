namespace Kurisu.AkiBT.DSL
{
    // expr `PropertyType: ...`
    public class PropertyExprAST : ExprAST
    {
        public PropertyExprAST(PropertyInfo metaData, ExprAST exprAST)
        {
            ExprType = ExprType.PropertyExpr;
            MetaData = metaData;
            Value = exprAST;
        }
        public PropertyInfo MetaData { get; }
        public ExprAST Value { get; }
        public override ExprType ExprType { get; protected set; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitPropertyAST(this);
        }
    }
}