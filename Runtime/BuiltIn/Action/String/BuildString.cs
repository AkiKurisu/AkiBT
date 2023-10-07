using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Build value of string")]
    [AkiLabel("String : Build")]
    [AkiGroup("String")]
    public class BuildString : Action
    {
        [SerializeField]
        private List<SharedString> values;
        [SerializeField, ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            foreach (var value in values) InitVariable(value);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            for (int i = 0; i < values.Count; i++)
            {
                storeResult.Value += values[i].Value;
            }
            return Status.Success;
        }
    }
}
