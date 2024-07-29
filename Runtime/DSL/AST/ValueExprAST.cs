namespace Kurisu.AkiBT.DSL
{
    /// <summary>
    /// Support field type and also compatible with SharedVariable's type
    /// </summary>
    public enum FieldType
    {
        Int,
        Float,
        Bool,
        Vector2,
        Vector2Int,
        Vector3,
        Vector3Int,
        String,
        // UnityEngine.Object
        Object,
        Enum,
        Variable,
        Unknown
    }
    public class ValueExprAST : ExprAST
    {
        public ValueExprAST(FieldType valueType, object value)
        {
            ExprType = ExprType.ValueExpr;
            Type = valueType;
            Value = value;
        }
        // Quick type query by property against to Type.GetType()
        public FieldType Type { get; }
        // Boxing value
        // TODO: Use generic expr implement for unmanaged value type
        public object Value { get; }
        public override ExprType ExprType { get; protected set; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitValueExprAST(this);
        }
        public static ValueExprAST String(string value)
        {
            return new ValueExprAST(FieldType.String, value);
        }
    }
}