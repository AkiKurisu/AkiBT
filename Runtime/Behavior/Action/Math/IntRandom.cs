using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Int type set random value")]
    [AkiLabel("Math: IntRandom")]
    [AkiGroup("Math")]
    public class IntRandom : Action
    {
        public enum Operation
        {
            Absolutely,
            Relatively
        }
        public Vector2Int range = new(-5, 5);
        public Operation operation;
        [ForceShared]
        public SharedInt randomInt;
        protected override Status OnUpdate()
        {
            int random = UnityEngine.Random.Range(range.x, range.y);
            randomInt.Value = (operation == Operation.Absolutely ? 0 : randomInt.Value) + random;
            return Status.Success;
        }
    }
}