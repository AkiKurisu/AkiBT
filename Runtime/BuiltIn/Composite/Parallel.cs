using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    [AkiInfo("Composite:平行,运行所有子结点,如果返回错误则退出返回Failure,否则等待所有子结点同时返回正确;"+
    "注意:Parallel和Sequence都会按子结点的顺序进行遍历,但Sequence会在子结点出现Running时进行等待,之后的结点就不会被Update,"+
    "而Parallel始终会运行所有结点"
    )]
    [AkiLabel("Parallel平行")]
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
                }else if (result == Status.Failure)
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
            runningNodes.ForEach( e => e.Abort() );
            runningNodes.Clear();
        }

    }
}