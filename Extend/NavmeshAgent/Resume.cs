using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: Resume")]
    [AkiInfo("Action: Resumes the movement along the current path after a pause.")]
    public class Resume : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        public override void Awake()
        {
            if (agent.Value == null)
                agent.Value = GameObject.GetComponent<NavMeshAgent>();
        }
        protected override Status OnUpdate()
        {
            if (agent.Value == null)
            {
                return Status.Failure;
            }
            agent.Value.isStopped = false;
            return Status.Success;
        }
    }
}