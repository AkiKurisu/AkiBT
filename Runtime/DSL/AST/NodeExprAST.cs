using System.Collections.Generic;
namespace Kurisu.AkiBT.DSL
{
    // expr `NodeType(...)`
    public class NodeExprAST : ExprAST
    {
        public NodeExprAST(NodeInfo metaData, List<PropertyExprAST> propertyExprASTs)
        {
            ExprType = ExprType.NodeExpr;
            MetaData = metaData;
            Properties = propertyExprASTs;
        }
        public NodeInfo MetaData { get; }
        public List<PropertyExprAST> Properties { get; }
        public override ExprType ExprType { get; protected set; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitNodeExprAST(this);
        }
    }
}