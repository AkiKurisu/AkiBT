using Kurisu.AkiBT.Extend;
using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.AkiBT.Example.Builder
{

    public class ExampleBuilder : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        public void Build()
        {
            var builder = new BehaviorTreeBuilder();
            builder.NewObject("NavAgent", GetComponent<NavMeshAgent>());
            builder
            .BeginChild(new Sequence())
                .Append(new TransformGetPosition() { target = new(target), storeResult = builder.NewVector3("TargetPosition") })
                .Append(new TransformGetPosition() { storeResult = builder.NewVector3("MyPosition") })
                .Append(new Vector3Distance()
                {
                    firstVector3 = builder.NewVector3("TargetPosition"),
                    secondVector3 = builder.NewVector3("MyPosition"),
                    storeResult = builder.NewFloat("Distance")
                })
                .BeginChild(new Selector())
                    .Append(new FloatComparison()
                    {
                        operation = FloatComparison.Operation.GreaterThan,
                        float1 = builder.NewFloat("Distance"),
                        float2 = new(4f)
                    })
                    .BeginChild()
                        .BeginChild(new Sequence())
                            .Append(new NavmeshSetDestination()
                            {
                                agent = builder.NewObject<NavMeshAgent>("NavAgent"),
                                destination = builder.NewVector3("TargetPosition")
                            })
                            .Append(new NavmeshStopAgent()
                            {
                                agent = builder.NewObject<NavMeshAgent>("NavAgent"),
                                isStopped = new(false)
                            })
                        .EndChild()
                    .EndChild()
                    .Append(new NavmeshStopAgent()
                    {
                        agent = builder.NewObject<NavMeshAgent>("NavAgent"),
                        isStopped = new(true)
                    })
                .EndChild()
            .EndChild()
            .Build(gameObject, out BehaviorTree _);
        }
    }
}