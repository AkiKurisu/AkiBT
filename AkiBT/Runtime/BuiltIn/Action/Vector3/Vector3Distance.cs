using UnityEngine;
using UnityEngine.Serialization;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:计算两个Vector3间的距离")]
    [AkiLabel("Vector3:Distance")]
    [AkiGroup("Vector3")]
    public class Vector3Distance : Action
    {
        [SerializeField]
        public SharedVector3 firstVector3;
        [SerializeField]
        public SharedVector3 secondVector3;
        [SerializeField,ForceShared,FormerlySerializedAs("result")]
        private SharedFloat storeResult;
        public override void Awake() {
            InitVariable(firstVector3);
            InitVariable(secondVector3);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            storeResult.Value=Vector3.Distance(firstVector3.Value,secondVector3.Value);
            return Status.Success;
        }
    }
}
