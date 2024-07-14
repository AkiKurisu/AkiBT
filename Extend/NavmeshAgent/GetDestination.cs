using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: GetDestination")]
    [AkiInfo("Action: Gets or attempts to set the destination of the agent in world-space units.")]
    public class GetDestination : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [ForceShared, Tooltip("The NavMeshAgent destination")]
        public SharedVector3 storeValue;
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
            storeValue.Value = agent.Value.destination;
            return Status.Success;
        }
    }
}