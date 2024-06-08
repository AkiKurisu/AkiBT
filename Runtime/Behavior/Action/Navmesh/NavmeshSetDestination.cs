using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Set the destination of NavmeshAgent")]
    [AkiLabel("Navmesh: SetDestination")]
    [AkiGroup("Navmesh")]
    public class NavmeshSetDestination : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        public SharedVector3 destination;
        protected override Status OnUpdate()
        {
            if (agent != null)
            {
                agent.Value.destination = destination.Value;
            }
            return Status.Success;
        }
        public override void Awake()
        {
            if (agent.Value == null) agent.Value = GameObject.GetComponent<NavMeshAgent>();
        }
    }
}
