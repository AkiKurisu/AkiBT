using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Float类型值运算")]
    [AkiLabel("Math:FloatOperator")]
    [AkiGroup("Math")]
    public class FloatOperator : Action
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
        private SharedFloat float1;
        [SerializeField]
        private SharedFloat float2;
        [SerializeField]
        private SharedFloat storeResult;
        [SerializeField]
        private Operation operation;
        
            
      
         public override void Awake() {
            float1.GetValueFromTree<float>(tree);
            float2.GetValueFromTree<float>(tree);
            storeResult.GetValueFromTree<float>(tree);     
        }
        public override void Abort()
        {
            operation = Operation.Add;
            float1.Value = 0;
            float2.Value= 0;
            storeResult.Value= 0;
        }
        protected override Status OnUpdate()
        {
            switch (operation) {
                case Operation.Add:
                    storeResult.Value = float1.Value + float2.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = float1.Value - float2.Value;
                    break;
                case Operation.Multiply:
                    storeResult.Value = float1.Value * float2.Value;
                    break;
                case Operation.Divide:
                    storeResult.Value = float1.Value / float2.Value;
                    break;
                case Operation.Min:
                    storeResult.Value = Mathf.Min(float1.Value, float2.Value);
                    break;
                case Operation.Max:
                    storeResult.Value = Mathf.Max(float1.Value, float2.Value);
                    break;
                case Operation.Modulo:
                    storeResult.Value = float1.Value % float2.Value;
                    break;
            }
            return Status.Success;
        }
    }
}