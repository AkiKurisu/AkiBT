using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Set bool value")]
    [AkiLabel("Math: SetBool")]
    [AkiGroup("Math")]
    public class SetBool : Action
    {
        public SharedBool boolValue;
        [ForceShared, FormerlySerializedAs("boolToSet")]
        public SharedBool storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = boolValue.Value;
            return Status.Success;
        }
    }
}