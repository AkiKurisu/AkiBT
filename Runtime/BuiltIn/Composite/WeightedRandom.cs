using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiInfo("Composite : Weighted random, randomly selected according to the weight" +
   ", wait for the node to finish running and reselect the next node")]
    public class WeightedRandom : Composite
    {
        private NodeBehavior runningNode;
        [SerializeField, Tooltip("Node weight list, when the length of the list is greater than the number of child nodes" +
        ", the excess part will not be included in the weight")]
        private List<float> weights = new();
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
            int count = Mathf.Min(weights.Count, Children.Count);
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
            if (runningNode != null)
            {
                runningNode.Abort();
                runningNode = null;
            }
        }
    }
}