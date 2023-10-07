using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Int type takes absolute value")]
    [AkiLabel("Math : IntAbs")]
    [AkiGroup("Math")]
    public class IntAbs : Action
    {
        [SerializeField, ForceShared]
        private SharedInt value;
        public override void Awake()
        {
            InitVariable(value);
        }
        protected override Status OnUpdate()
        {
            value.Value = Mathf.Abs(value.Value);
            return Status.Success;
        }
    }
}
