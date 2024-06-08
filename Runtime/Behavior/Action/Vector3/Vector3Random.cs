using UnityEngine;
using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Random get Vector3 value")]
    [AkiLabel("Vector3: Random")]
    [AkiGroup("Vector3")]
    public class Vector3Random : Action
    {
        public enum Operation
        {
            Absolutely,
            Relatively
        }

        public Vector2 xRange = new(-5, 5);
        public Vector2 yRange = new(-5, 5);
        public Vector2 zRange = new(-5, 5);
        public Operation operation;
        [ForceShared, FormerlySerializedAs("randomVector3")]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            Vector3 addVector3 = new(UnityEngine.Random.Range(xRange.x, xRange.y), UnityEngine.Random.Range(yRange.x, yRange.y), UnityEngine.Random.Range(zRange.x, zRange.y));
            storeResult.Value = (operation == Operation.Absolutely ? Vector3.zero : storeResult.Value) + addVector3;
            return Status.Success;
        }
    }
}