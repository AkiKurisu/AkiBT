using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: IsStopped")]
    [AkiInfo("Conditional: Whether NavMeshAgent is stopped?")]
    public class IsStopped : Conditional
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;

        protected override bool IsUpdatable()
        {
            if (agent.Value != null)
            {
                return agent.Value.isStopped;
            }
            return false;
        }

        protected override void OnAwake()
        {
            if (agent.Value == null)
                agent.Value = GameObject.GetComponent<NavMeshAgent>();
        }
    }
}