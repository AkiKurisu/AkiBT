using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Operate bool value")]
    [AkiLabel("Math : BoolOperator")]
    [AkiGroup("Math")]
    public class BoolOperator : Action
    {
        private enum Operation
        {
            And,
            Or
        }
        [SerializeField]
        private SharedBool bool1;
        [SerializeField]
        private SharedBool bool2;
        [SerializeField, ForceShared]
        private SharedBool storeResult;
        [SerializeField]
        private Operation operation;
        public override void Awake()
        {
            InitVariable(bool1);
            InitVariable(bool2);
            InitVariable(storeResult);
        }
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