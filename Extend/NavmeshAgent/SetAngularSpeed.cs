using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend.UnityNavMeshAgent
{
    [AkiGroup("NavMeshAgent")]
    [AkiLabel("NavMeshAgent: SetAngularSpeed")]
    [AkiInfo("Action: Set NavMeshAgent's maximum turning speed in (deg/s) while following a path.")]
    public class SetAngularSpeed : Action
    {
        [Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        public SharedTObject<NavMeshAgent> agent;
        public SharedFloat angularSpeed;
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
            agent.Value.angularSpeed = angularSpeed.Value;
            return Status.Success;
        }
    }
}