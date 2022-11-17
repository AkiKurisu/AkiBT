using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Vector3类型值运算")]
    [AkiLabel("Vector:Operator")]
    [AkiGroup("Vector")]
public class Vector3Operator : Action
{
    
        public enum Operation
        {
            Add,
            Subtract,
            Scale
        }

        public Operation operation;
        public SharedVector3 firstVector3=new SharedVector3();
        public SharedVector3 secondVector3=new SharedVector3();
        public SharedVector3 storeResult=new SharedVector3();
        public override void Awake() {

            firstVector3.GetValueFromTree<Vector3>(tree);
            secondVector3.GetValueFromTree<Vector3>(tree);
            storeResult.GetValueFromTree<Vector3>(tree);

        }
        protected override Status OnUpdate()
        {
            switch (operation) {
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

        public override void Abort()
        {
            operation = Operation.Add;
            firstVector3.Value = Vector3.zero; 
            secondVector3.Value = Vector3.zero; 
            storeResult.Value = Vector3.zero;
        }
}
}