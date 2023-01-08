using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
     [AkiInfo("Composite:加权随机,根据权重随机选择,等待结点结束运行后重新选择下一个结点")]
     [AkiLabel("WeightedRandom加权随机")]
    public class WeightedRandom : Composite
    {
        private NodeBehavior runningNode;
        [SerializeField,Tooltip("结点权重列表,列表长度大于子结点数量时,超过部分不会计入权重")]
        private List<float> weights=new List<float>();
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
            int count=Mathf.Min(weights.Count,Children.Count);
            for (int i = 0; i < count; i++)
            {
                total += weights[i];
            }
            float random = UnityEngine.Random.Range(0,total);
            
            for (int i = 0; i < count; i++)
            {
                if (random < weights[i])return i;
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