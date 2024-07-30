using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Set float value")]
    [AkiLabel("Math: SetFloat")]
    [AkiGroup("Math")]
    public class SetFloat : Action
    {
        public SharedFloat floatValue;
        [ForceShared, FormerlySerializedAs("floatToSet")]
        public SharedFloat storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = floatValue.Value;
            return Status.Success;
        }
    }
}