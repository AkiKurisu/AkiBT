using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kurisu.AkiBT;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
[AkiInfo("Action:根据isStopped停止NavmeshAgent")]
[AkiLabel("Navmesh:StopAgent")]
[AkiGroup("Navmesh")]
public class NavmeshStopAgent : Action
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private bool isStopped;
    protected override Status OnUpdate()
    {
        if(_navMeshAgent!=null&&_navMeshAgent.isStopped!=isStopped)
        {
            _navMeshAgent.isStopped=isStopped;
        }
        return Status.Success;
    }
    public override void Awake()
    {
        _navMeshAgent=gameObject.GetComponent<NavMeshAgent>();
    }
    
    }
}
