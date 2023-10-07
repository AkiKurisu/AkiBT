using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Convert float type to int type")]
    [AkiLabel("Math : Float2Int")]
    [AkiGroup("Math")]
    public class Float2Int : Action
    {
        [SerializeField]
        private SharedFloat value;
        [SerializeField, ForceShared]
        private SharedInt storeResult;
        public override void Awake()
        {
            InitVariable(value);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = (int)value.Value;
            return Status.Success;
        }
    }
}
