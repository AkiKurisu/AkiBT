using UnityEngine;

namespace Kurisu.AkiBT
{
    [AkiInfo("Composite:序列,依次遍历子结点,若返回Success则继续更新下一个,否则返回Failure")]
    [AkiLabel("Sequence序列")]
    public class Sequence : Composite
    {
        [SerializeField,AkiLabel("在判断改变时打断子结点"),Tooltip("当优先于当前运行结点的子结点判断改变时打断当前运行结点,打断会影响其分支下全部结点;注意:在AkiBT中,Action结点的判断始终"+
        "为true,只有Conditional结点会发生判断(CanUpdate)的改变"
        )]
        private bool abortOnConditionChanged = true;

        private NodeBehavior runningNode;

        public override bool CanUpdate()
        {
            //this node can update when all children can update
            foreach (var child in Children)
            {
                if (!child.CanUpdate())
                {
                    return false;
                }
            }
            return true;
        }

        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                if (abortOnConditionChanged && IsConditionChanged(runningNode))
                {
                    runningNode.Abort();
                    return UpdateWhileSuccess(0);
                }

                var currentOrder = Children.IndexOf(runningNode);
                var status = runningNode.Update();
                if (status == Status.Success)
                {
                    // update next nodes
                    return UpdateWhileSuccess(currentOrder + 1);
                }

                return HandleStatus(status, runningNode);
            }

            return UpdateWhileSuccess(0);

        }

        private bool IsConditionChanged(NodeBehavior runningChild)
        {
            // when the conditions of a node with a higher priority than itself can not update.
            var priority = Children.IndexOf(runningChild);
            for (var i = 0; i < priority; i++)
            {
                var candidate = Children[i];
                if (!candidate.CanUpdate())
                {
                    return true;
                }
            }
            return false;
        }

        private Status UpdateWhileSuccess(int start)
        {
            for (var i = start; i < Children.Count; i++)
            {
                var target = Children[i];
                var childStatus = target.Update();
                if (childStatus == Status.Success)
                {
                    continue;
                }
                return HandleStatus(childStatus, target);
            }

            return HandleStatus(Status.Success, null);
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