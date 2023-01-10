using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Extend
{
[AkiInfo("Action:设置NavmeshAgent的目的地")]
[AkiLabel("Navmesh:SetDestination")]
[AkiGroup("Navmesh")]
public class NavmeshSetDestination : Action
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private SharedVector3 destination;
    protected override Status OnUpdate()
    {
        if(_navMeshAgent!=null)
        {
            _navMeshAgent.destination=destination.Value;
        }
        return Status.Success;
    }
    public override void Awake()
    {
        _navMeshAgent=gameObject.GetComponent<NavMeshAgent>();
        InitVariable(destination);
    }
    
    }
}
