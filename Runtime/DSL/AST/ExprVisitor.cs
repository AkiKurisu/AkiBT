using UnityEngine.Assertions;
namespace Kurisu.AkiBT.DSL
{
    public abstract class ExprVisitor
    {
        protected ExprVisitor()
        {
        }

        public virtual ExprAST Visit(ExprAST node)
        {
            if (node != null)
            {
                return node.Accept(this);
            }

            return null;
        }

        protected internal virtual ExprAST VisitExtension(ExprAST node)
        {
            return node.VisitChildren(this);
        }
        protected internal virtual ExprAST VisitArrayExprAST(ArrayExprAST node)
        {
            foreach (var child in node.Children)
            {
                Visit(child);
            }
            return node;
        }

        protected internal virtual ExprAST VisitNodeExprAST(NodeExprAST node)
        {
            foreach (var property in node.Properties)
            {
                Visit(property);
            }
            return node;
        }

        protected internal virtual ExprAST VisitPropertyAST(PropertyExprAST node)
        {
            Visit(node.Value);
            return node;
        }

        protected internal virtual ExprAST VisitVariableExprAST(VariableExprAST node)
        {
            Visit(node.Value);
            return node;
        }

        protected internal virtual ExprAST VisitVariableDefineAST(VariableDefineExprAST node)
        {
            Visit(node.Value);
            return node;
        }
        protected internal virtual ExprAST VisitObjectDefineAST(ObjectDefineExprAST node)
        {
            Visit(node.Value);
            return node;
        }

        protected internal virtual ExprAST VisitValueExprAST(ValueExprAST node)
        {
            // Should not happen
            Assert.IsFalse(node.Value is ExprAST);
            return node;
        }
    }
}
