namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition: Compare Int values, if the conditions are met, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Math: IntComparison")]
    [AkiGroup("Math")]
    public class IntComparison : Conditional
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
        public SharedInt int1;
        public SharedInt int2;
        public Operation operation;
        protected override bool IsUpdatable()
        {
            return operation switch
            {
                Operation.LessThan => int1.Value < int2.Value,
                Operation.LessThanOrEqualTo => int1.Value <= int2.Value,
                Operation.EqualTo => int1.Value == int2.Value,
                Operation.NotEqualTo => int1.Value != int2.Value,
                Operation.GreaterThanOrEqualTo => int1.Value >= int2.Value,
                Operation.GreaterThan => int1.Value > int2.Value,
                _ => true,
            };
        }
    }
}