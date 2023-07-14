using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Float类型转为Int类型")]
    [AkiLabel("Math:Float2Int")]
    [AkiGroup("Math")]
    public class Float2Int : Action
    {
        [SerializeField]
        private SharedFloat value;
        [SerializeField,ForceShared]
        private SharedInt storeResult;
        public override void Awake() {
            InitVariable(value);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value=(int)value.Value;
            return Status.Success;
        }
    }
}
