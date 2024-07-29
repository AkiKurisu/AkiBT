using System.Collections.Generic;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Set random string value")]
    [AkiLabel("String: Random")]
    [AkiGroup("String")]
    public class StringRandom : Action
    {
        public List<string> randomStrings;
        [ForceShared]
        public SharedString storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = randomStrings[UnityEngine.Random.Range(0, randomStrings.Count)];
            return Status.Success;
        }
    }
}
