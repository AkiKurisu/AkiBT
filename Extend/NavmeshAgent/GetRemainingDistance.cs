using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: GetRemainingDistance")]
    [AkiInfo("Action: Get the distance between the agent's position and the destination on the current path.")]
    public class GetRemainingDistance : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [ForceShared, Tooltip("The NavMeshAgent remainingDistance")]
        public SharedFloat storeValue;
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
            storeValue.Value = agent.Value.remainingDistance;
            return Status.Success;
        }
    }
}