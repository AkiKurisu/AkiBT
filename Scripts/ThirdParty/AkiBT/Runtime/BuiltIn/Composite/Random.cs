namespace Kurisu.AkiBT
{
     [AkiInfo("Composite:随机,等待结点结束运行后重新选择下一个结点")]
     [AkiLabel("Random随机")]
    public class Random : Composite
    {
        private NodeBehavior runningNode;

        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                return HandleStatus(runningNode.Update(), runningNode);
            }
            
            var result = UnityEngine.Random.Range(0, Children.Count);
            var target = Children[result];
            return HandleStatus(target.Update(), target);
        }

        private Status HandleStatus(Status status, NodeBehavior updated)
        {
            runningNode = status == Status.Running ? updated : null;
            return status;
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