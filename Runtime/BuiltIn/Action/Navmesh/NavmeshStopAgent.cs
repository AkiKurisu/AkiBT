using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Stop NavmeshAgent according to isStopped")]
    [AkiLabel("Navmesh : StopAgent")]
    [AkiGroup("Navmesh")]
    public class NavmeshStopAgent : Action
    {
        [SerializeField, Tooltip("If not filled in, it will be obtained from the bound gameObject")]
        private SharedTObject<NavMeshAgent> agent;
        [SerializeField, Tooltip("Whether to stop")]
        private SharedBool isStopped;
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
            InitVariable(agent);
            if (agent.Value == null) agent.Value = GameObject.GetComponent<NavMeshAgent>();
            InitVariable(isStopped);
        }
    }
}
