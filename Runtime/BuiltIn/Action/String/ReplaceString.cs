using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Replace value of string")]
    [AkiLabel("String : Replace")]
    [AkiGroup("String")]
    public class ReplaceString : Action
    {
        [SerializeField]
        private SharedString target;
        [SerializeField]
        private SharedString replaceFrom;
        [SerializeField]
        private SharedString replaceTo;
        [SerializeField, ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            InitVariable(target);
            InitVariable(replaceFrom);
            InitVariable(replaceTo);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = target.Value.Replace(replaceFrom.Value, replaceTo.Value);
            return Status.Success;
        }
    }
}
