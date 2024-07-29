using System;
using System.Collections.Generic;
namespace Kurisu.AkiBT.DSL
{
    // expr `[..., ..., ...]`
    public class ArrayExprAST : ExprAST
    {
        public ArrayExprAST(Type fieldType, List<ExprAST> exprASTs)
        {
            ExprType = ExprType.ArrayExpr;
            FieldType = fieldType;
            Children = exprASTs;
        }
        public List<ExprAST> Children { get; }
        public Type FieldType { get; }
        public override ExprType ExprType { get; protected set; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitArrayExprAST(this);
        }
    }
}