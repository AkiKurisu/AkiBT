using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiInfo("Composite: Weighted random, randomly selected according to the weight" +
   ", wait for the node to finish running and reselect the next node")]
    public class WeightedRandom : Composite
    {
        private NodeBehavior runningNode;
        [Tooltip("Children weights, when weights length is greater than children count" +
        ", the excess part will be ignored")]
        public float[] weights;
        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                return HandleStatus(runningNode.Update(), runningNode);
            }
            var result = GetNext();
            var target = Children[result];
            return HandleStatus(target.Update(), target);
        }

        private Status HandleStatus(Status status, NodeBehavior updated)
        {
            runningNode = status == Status.Running ? updated : null;
            return status;
        }
        private int GetNext()
        {
            float total = 0;
            int count = Mathf.Min(weights.Length, Children.Count);
            for (int i = 0; i < count; i++)
            {
                total += weights[i];
            }
            float random = UnityEngine.Random.Range(0, total);

            for (int i = 0; i < count; i++)
            {
                if (random < weights[i]) return i;
                random -= weights[i];
            }
            return 0;
        }

        public override void Abort()
        {
            runningNode?.Abort();
            runningNode = null;
        }
    }
}