using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiInfo("Composite : Rotator, update child nodes in order, each Update will only update the current node" +
    ", after the node finishes running, the next Update will continue to update the next node")]
    public class Rotator : Composite
    {
        [SerializeField]
        private bool resetOnAbort;

        private int targetIndex;

        private NodeBehavior runningNode;

        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                return HandleStatus(runningNode.Update(), runningNode);
            }
            return HandleStatus(Children[targetIndex].Update(), Children[targetIndex]);
        }

        private void SetNext()
        {
            targetIndex++;
            if (targetIndex >= Children.Count)
            {
                targetIndex = 0;
            }
        }

        private Status HandleStatus(Status status, NodeBehavior updated)
        {
            if (status == Status.Running)
            {
                runningNode = updated;
            }
            else
            {
                runningNode = null;
                SetNext();
            }
            return status;
        }

        public override void Abort()
        {
            if (runningNode != null)
            {
                runningNode.Abort();
                runningNode = null;
            }
            if (resetOnAbort)
            {
                targetIndex = 0;
            }
        }
    }
}