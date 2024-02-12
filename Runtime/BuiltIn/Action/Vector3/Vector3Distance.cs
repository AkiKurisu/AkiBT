using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Calculate the distance between two Vector3")]
    [AkiLabel("Vector3: Distance")]
    [AkiGroup("Vector3")]
    public class Vector3Distance : Action
    {
        public SharedVector3 firstVector3;
        public SharedVector3 secondVector3;
        [ForceShared]
        public SharedFloat storeResult;
        protected override Status OnUpdate()
        {
            storeResult.Value = Vector3.Distance(firstVector3.Value, secondVector3.Value);
            return Status.Success;
        }
    }
}
