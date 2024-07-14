using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: GetAngularSpeed")]
    [AkiInfo("Action: Maximum turning speed in (deg/s) while following a path.")]
    public class GetAngularSpeed : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        [ForceShared, Tooltip("The NavMeshAgent angularSpeed")]
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
            storeValue.Value = agent.Value.angularSpeed;
            return Status.Success;
        }
    }
}