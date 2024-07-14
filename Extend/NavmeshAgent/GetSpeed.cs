using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: GetSpeed")]
    [AkiInfo("Action: Get the maximum movement speed when following a path.")]
    public class GetSpeed : Action
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
            storeValue.Value = agent.Value.speed;
            return Status.Success;
        }
    }
}