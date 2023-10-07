using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Set random string value")]
    [AkiLabel("String : Random")]
    [AkiGroup("String")]
    public class StringRandom : Action
    {
        [SerializeField]
        private List<string> randomStrings;
        [SerializeField, ForceShared]
        private SharedString storeResult;
        public override void Awake()
        {
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value = randomStrings[UnityEngine.Random.Range(0, randomStrings.Count)];
            return Status.Success;
        }
    }
}
