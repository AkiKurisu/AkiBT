using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Operate Vector3 value")]
    [AkiLabel("Vector3 : Operator")]
    [AkiGroup("Vector3")]
    public class Vector3Operator : Action
    {

        public enum Operation
        {
            Add,
            Subtract,
            Scale
        }
        [SerializeField]
        private Operation operation;
        [SerializeField]
        private SharedVector3 firstVector3;
        [SerializeField]
        private SharedVector3 secondVector3;
        [SerializeField, ForceShared]
        private SharedVector3 storeResult;
        public override void Awake()
        {
            InitVariable(firstVector3);
            InitVariable(secondVector3);
            InitVariable(storeResult);
        }
        protected override Status OnUpdate()
        {
            switch (operation)
            {
                case Operation.Add:
                    storeResult.Value = firstVector3.Value + secondVector3.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = firstVector3.Value - secondVector3.Value;
                    break;
                case Operation.Scale:
                    storeResult.Value = Vector3.Scale(firstVector3.Value, secondVector3.Value);
                    break;
            }
            return Status.Success;
        }
    }
}