using UnityEngine;
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
        [SerializeField]
        private SharedFloat result;
        public override void Awake() {
            InitVariable(firstVector3);
            InitVariable(secondVector3);
            InitVariable(result);
        }
        protected override Status OnUpdate()
        {
            result.Value=Vector3.Distance(firstVector3.Value,secondVector3.Value);
            return Status.Success;
        }
    }
}
