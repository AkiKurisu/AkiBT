using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:计算Vector3模的平方,性能优于Distance,但会损失精度")]
    [AkiLabel("Vector3:GetSqrMagnitude")]
    [AkiGroup("Vector3")]
    public class Vector3GetSqrMagnitude : Action
    {
        [SerializeField,Tooltip("待计算的数值")]
        public SharedVector3 vector3;
        [SerializeField]
        private SharedFloat result;
        public override void Awake() {
            InitVariable(vector3);
            InitVariable(result);
        }
        protected override Status OnUpdate()
        {
            result.Value=vector3.Value.sqrMagnitude;
            return Status.Success;
        }
    }
}
