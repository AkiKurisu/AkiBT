using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition : Compare Float values, if the condition is met, return Status.Success, otherwise return Status.Failure")]
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
        [SerializeField]
        private SharedFloat float1;
        [SerializeField]
        private SharedFloat float2;
        [SerializeField]
        private Operation operation;
        protected override void OnStart()
        {
            InitVariable(float1);
            InitVariable(float2);
        }
        protected override bool IsUpdatable()
        {
            switch (operation)
            {
                case Operation.LessThan:
                    return float1.Value < float2.Value ? true : false;
                case Operation.LessThanOrEqualTo:
                    return float1.Value <= float2.Value ? true : false;
                case Operation.EqualTo:
                    return float1.Value == float2.Value ? true : false;
                case Operation.NotEqualTo:
                    return float1.Value != float2.Value ? true : false;
                case Operation.GreaterThanOrEqualTo:
                    return float1.Value >= float2.Value ? true : false;
                case Operation.GreaterThan:
                    return float1.Value > float2.Value ? true : false;
            }
            return true;
        }
    }
}