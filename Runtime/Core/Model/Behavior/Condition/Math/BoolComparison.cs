namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition: Compare Bool values, if the conditions are met, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Math: BoolComparison")]
    [AkiGroup("Math")]
    public class BoolComparison : Conditional
    {
        public enum Operation
        {
            EqualTo,
            NotEqualTo,
        }
        public SharedBool bool1;
        public SharedBool bool2;
        public Operation operation;
        protected override bool IsUpdatable()
        {
            return operation switch
            {
                Operation.EqualTo => bool1.Value == bool2.Value,
                Operation.NotEqualTo => bool1.Value != bool2.Value,
                _ => true,
            };
        }
    }
}