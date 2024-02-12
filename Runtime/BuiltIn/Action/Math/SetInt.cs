using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Set int value")]
    [AkiLabel("Math: SetInt")]
    [AkiGroup("Math")]
    public class SetInt : Action
    {
        public SharedInt intValue;
        [ForceShared, FormerlySerializedAs("intToSet")]
        public SharedInt storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = intValue.Value;
            return Status.Success;
        }
    }
}