using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Operate Vector3 value")]
    [AkiLabel("Vector3: Operator")]
    [AkiGroup("Vector3")]
    public class Vector3Operator : Action
    {

        public enum Operation
        {
            Add,
            Subtract,
            Scale
        }
        public Operation operation;
        public SharedVector3 firstVector3;
        public SharedVector3 secondVector3;
        [ForceShared]
        public SharedVector3 storeResult;
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