using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:随机获取Vector3值")]
    [AkiLabel("Vector3:Random")]
    [AkiGroup("Vector3")]
    public class Vector3Random : Action
    {
        private enum Operation
        {
            Absolutely,
            Relatively
        }
        [SerializeField]
        private SharedVector3 randomVector3;
        [SerializeField]
        private Vector2 xRange=new Vector2(-5,5);
        [SerializeField]
        private Vector2 yRange=new Vector2(-5,5);
        [SerializeField]
        private Vector2 zRange=new Vector2(-5,5);
        [SerializeField]
        private Operation operation;
        public override void Awake()
        {
           InitVariable(randomVector3);
        }
        protected override Status OnUpdate()
        {
            Vector3 addVector3=new Vector3(UnityEngine.Random.Range(xRange.x,xRange.y),UnityEngine.Random.Range(yRange.x,yRange.y),UnityEngine.Random.Range(zRange.x,zRange.y));
            randomVector3.Value=(operation==Operation.Absolutely?Vector3.zero:randomVector3.Value)+addVector3;
            return Status.Success;
        }
    }
}