using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:String类型赋值")]
    [AkiLabel("String:Set")]
    [AkiGroup("String")]
    public class SetString : Action
    {
        [SerializeField]
        private SharedString value;
        [SerializeField,ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            InitVariable(value);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value=value.Value;
            return Status.Success;
        }
    }
}
