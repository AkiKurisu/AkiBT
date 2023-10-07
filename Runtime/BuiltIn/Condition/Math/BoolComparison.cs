using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition : Compare Bool values, if the conditions are met, return Status.Success, otherwise return Status.Failure")]
    [AkiLabel("Math : BoolComparison")]
    [AkiGroup("Math")]
    public class BoolComparison : Conditional
    {
        private enum Operation
        {
            EqualTo,
            NotEqualTo,
        }
        [SerializeField]
        private SharedBool bool1;
        [SerializeField]
        private SharedBool bool2;
        [SerializeField]
        private Operation operation;
        protected override void OnStart()
        {
            InitVariable(bool1);
            InitVariable(bool2);
        }
        protected override bool IsUpdatable()
        {
            switch (operation)
            {
                case Operation.EqualTo:
                    return bool1.Value == bool2.Value;
                case Operation.NotEqualTo:
                    return bool1.Value != bool2.Value;

            }
            return true;
        }
    }
}