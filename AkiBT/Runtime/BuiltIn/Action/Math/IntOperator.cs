using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Int类型值运算")]
    [AkiLabel("Math:IntOperator")]
    [AkiGroup("Math")]
    public class IntOperator : Action
    {
        private enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
            Modulo
        }
        [SerializeField]
        private SharedInt int1=new SharedInt();
        [SerializeField]
        private SharedInt int2=new SharedInt();
        [SerializeField]
        private SharedInt storeResult=new SharedInt();
        [SerializeField]
        private Operation operation;
        
            
      
        public override void Awake() {
            int1.GetValueFromTree<int>(tree);
            int2.GetValueFromTree<int>(tree);
            storeResult.GetValueFromTree<int>(tree);
        }
        public override void Abort()
        {
            operation = Operation.Add;
            int1.Value = 0;
            int2.Value= 0;
            storeResult.Value= 0;
        }
        protected override Status OnUpdate()
        {
            switch (operation) {
                case Operation.Add:
                    storeResult.Value = int1.Value + int2.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = int1.Value - int2.Value;
                    break;
                case Operation.Multiply:
                    storeResult.Value = int1.Value * int2.Value;
                    break;
                case Operation.Divide:
                    storeResult.Value = int1.Value / int2.Value;
                    break;
                case Operation.Min:
                    storeResult.Value = Mathf.Min(int1.Value, int2.Value);
                    break;
                case Operation.Max:
                    storeResult.Value = Mathf.Max(int1.Value, int2.Value);
                    break;
                case Operation.Modulo:
                    storeResult.Value = int1.Value % int2.Value;
                    break;
            }
            return Status.Success;
        }
    }
}