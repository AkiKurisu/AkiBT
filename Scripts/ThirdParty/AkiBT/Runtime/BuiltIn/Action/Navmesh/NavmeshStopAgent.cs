using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
[AkiInfo("Action:根据isStopped停止NavmeshAgent")]
[AkiLabel("Navmesh:StopAgent")]
[AkiGroup("Navmesh")]
public class NavmeshStopAgent : Action
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField,AkiLabel("是否停止")]
    private SharedBool isStopped;
    protected override Status OnUpdate()
    {
        if(_navMeshAgent!=null&&_navMeshAgent.isStopped!=isStopped.Value)
        {
            _navMeshAgent.isStopped=isStopped.Value;
        }
        return Status.Success;
    }
    public override void Awake()
    {
        _navMeshAgent=gameObject.GetComponent<NavMeshAgent>();
        InitVariable(isStopped);
    }
    
    }
}
