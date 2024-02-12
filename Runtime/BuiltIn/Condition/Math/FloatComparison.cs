namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition: Compare Float values, if the condition is met, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Math : FloatComparison")]
    [AkiGroup("Math")]
    public class FloatComparison : Conditional
    {
        public enum Operation
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            NotEqualTo,
            GreaterThanOrEqualTo,
            GreaterThan
        }
        public SharedFloat float1;
        public SharedFloat float2;
        public Operation operation;
        protected override bool IsUpdatable()
        {
            return operation switch
            {
                Operation.LessThan => float1.Value < float2.Value,
                Operation.LessThanOrEqualTo => float1.Value <= float2.Value,
                Operation.EqualTo => float1.Value == float2.Value,
                Operation.NotEqualTo => float1.Value != float2.Value,
                Operation.GreaterThanOrEqualTo => float1.Value >= float2.Value,
                Operation.GreaterThan => float1.Value > float2.Value,
                _ => true,
            };
        }
    }
}