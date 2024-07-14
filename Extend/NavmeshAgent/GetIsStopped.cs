using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: GetIsStopped")]
    [AkiInfo("Action: Get property holds the stop or resume condition of the NavMesh agent.")]
    public class GetIsStopped : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [ForceShared, Tooltip("The NavMeshAgent isStopped")]
        public SharedBool storeValue;
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
            storeValue.Value = agent.Value.isStopped;
            return Status.Success;
        }
    }
}