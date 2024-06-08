using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    [AkiInfo("Composite: run in parallel, run all sub-nodes, exit and return Failure if an error is returned, otherwise wait for all sub-nodes to return correct at the same time;" +
     "Note: Both Parallel and Sequence will traverse in the order of child nodes, but Sequence will wait when child node appears to be Running, and the subsequent nodes will not be updated," +
     "While Parallel always runs all nodes"
     )]
    public class Parallel : Composite
    {

        private List<NodeBehavior> runningNodes;

        protected override void OnAwake()
        {
            runningNodes = new List<NodeBehavior>();
        }

        /// <summary>
        /// Update all nodes.
        /// - any running -> Running
        /// - any failed -> Failure
        /// - else -> Success
        /// </summary>
        protected override Status OnUpdate()
        {
            runningNodes.Clear();
            var anyFailed = false;
            foreach (var c in Children)
            {
                var result = c.Update();
                if (result == Status.Running)
                {
                    runningNodes.Add(c);
                }
                else if (result == Status.Failure)
                {
                    anyFailed = true;
                }
            }
            if (runningNodes.Count > 0)
            {
                return Status.Running;
            }

            if (anyFailed)
            {
                runningNodes.ForEach(e => e.Abort());
                return Status.Failure;
            }

            return Status.Success;
        }

        public override void Abort()
        {
            runningNodes.ForEach(e => e.Abort());
            runningNodes.Clear();
        }

    }
}