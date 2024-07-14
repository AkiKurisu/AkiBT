using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: Warp")]
    [AkiInfo("Action: Warps agent to the provided position.")]
    public class Warp : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [Tooltip("The position warp to")]
        public SharedVector3 newPosition;
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
            agent.Value.Warp(newPosition.Value);
            return Status.Success;
        }
    }
}