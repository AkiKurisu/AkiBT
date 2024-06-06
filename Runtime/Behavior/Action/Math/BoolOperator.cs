namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Operate bool value")]
    [AkiLabel("Math: BoolOperator")]
    [AkiGroup("Math")]
    public class BoolOperator : Action
    {
        public enum Operation
        {
            And,
            Or
        }
        public SharedBool bool1;
        public SharedBool bool2;
        [ForceShared]
        public SharedBool storeResult;
        public Operation operation;
        protected override Status OnUpdate()
        {
            switch (operation)
            {
                case Operation.And:
                    storeResult.Value = bool1.Value && bool2.Value;
                    break;
                case Operation.Or:
                    storeResult.Value = bool1.Value || bool2.Value;
                    break;
            }
            return Status.Success;
        }
    }
}