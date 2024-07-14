using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Stop NavMeshAgent according to isStopped")]
    [AkiLabel("NavMeshAgent: StopAgent")]
    [AkiGroup("NavMeshAgent")]
    public class NavmeshStopAgent : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [Tooltip("Whether to stop")]
        public SharedBool isStopped;
        protected override Status OnUpdate()
        {
            if (agent.Value != null && agent.Value.isStopped != isStopped.Value)
            {
                agent.Value.isStopped = isStopped.Value;
            }
            return Status.Success;
        }
        public override void Awake()
        {
            if (agent.Value == null) agent.Value = GameObject.GetComponent<NavMeshAgent>();
        }
    }
}
