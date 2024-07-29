namespace Kurisu.AkiBT.DSL
{
    // expr `Vector3 myPos ...`
    // if global expr `$Vector3$ myPos ...`
    public class VariableDefineExprAST : ExprAST
    {
        public VariableDefineExprAST(FieldType variableType, bool isGlobal, string variableName, ValueExprAST valueExprAST)
        {
            ExprType = ExprType.VariableDefineExpr;
            Type = variableType;
            IsGlobal = isGlobal;
            Name = variableName;
            Value = valueExprAST;
        }
        public FieldType Type { get; }
        public bool IsGlobal { get; }
        public string Name { get; }
        public ValueExprAST Value { get; }
        public override ExprType ExprType { get; protected set; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitVariableDefineAST(this);
        }
    }
    // special case for SharedObject define
    // expr `Object navAgent "UnityEngine.AIModule,UnityEngine.AI.NavMeshAgent" Null`
    public class ObjectDefineExprAST : VariableDefineExprAST
    {
        public ObjectDefineExprAST(FieldType variableType, bool isGlobal, string variableName, string aqn, ValueExprAST valueExprAST)
        : base(variableType, isGlobal, variableName, valueExprAST)
        {
            ExprType = ExprType.ObjectDefineExpr;
            ConstraintTypeAQN = aqn;
        }
        public string ConstraintTypeAQN { get; }
        protected internal override ExprAST Accept(ExprVisitor visitor)
        {
            return visitor.VisitObjectDefineAST(this);
        }
    }
}