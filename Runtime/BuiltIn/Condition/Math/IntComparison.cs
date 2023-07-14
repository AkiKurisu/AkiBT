using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition:比较Int值,如果满足条件返回True,否则返回False")]
    [AkiLabel("Math:IntComparison")]
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
        [SerializeField]
        private SharedInt int1;
        [SerializeField]
        private SharedInt int2;
        [SerializeField]
        private Operation operation;
        protected override void OnStart()
        {
            InitVariable(int1);
            InitVariable(int2);
        }
        protected override bool IsUpdatable()
        {
            switch (operation) {
                case Operation.LessThan:
                    return int1.Value < int2.Value ? true : false;
                case Operation.LessThanOrEqualTo:
                    return int1.Value <= int2.Value ? true : false;
                case Operation.EqualTo:
                    return int1.Value == int2.Value ? true : false;
                case Operation.NotEqualTo:
                    return int1.Value != int2.Value ? true : false;
                case Operation.GreaterThanOrEqualTo:
                    return int1.Value >= int2.Value ? true : false;
                case Operation.GreaterThan:
                    return int1.Value > int2.Value ? true : false;
            }
            return true;
        }
    }
}