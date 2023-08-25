using UnityEngine;

namespace Kurisu.AkiBT
{
    [AkiInfo("Composite : Sequence, traversing the child nodes in turn, if it returns Success" +
     ", continue to update the next one, otherwise it returns Failure")]
    public class Sequence : Composite
    {
        [SerializeField, Tooltip("When the judgment of the child node prior to the current running node is changed," +
        " the current running node is interrupted, and the interruption will affect all nodes under its branches;" +
        " Note: in AkiBT, the judgment of the Action node is always" +
         "For true, only the Conditional node will change the judgment (CanUpdate)"
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