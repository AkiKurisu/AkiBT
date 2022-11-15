using UnityEngine;

namespace Kurisu.AkiBT
{
    /// <summary>
    ///  update the children in order.
    ///  update only one child per frame.
    /// </summary>
    [AkiInfo("Composite:轮盘,按顺序更新子结点,当结点结束运行后更新下一个结点")]
    [AkiLabel("Rotator轮盘")]
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

            var target = Children[targetIndex];
            return HandleStatus(target.Update(), target);
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