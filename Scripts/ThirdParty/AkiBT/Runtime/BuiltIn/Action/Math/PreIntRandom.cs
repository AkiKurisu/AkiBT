using UnityEngine;
namespace Kurisu.AkiBT
{
    [AkiInfo("Action:Int类型随机值,Pre版本会在Start时候赋值一次,运行时可以选择是否重新赋值")]
    [AkiLabel("Math:PreIntRandom")]
    [AkiGroup("Math")]
    public class PreIntRandom : Action
{
    private enum Operation
        {
            Absolutely,
            Relatively
        }
        [SerializeField]
        private SharedInt randomInt;
        [SerializeField]
        private Vector2Int range=new Vector2Int(-5,5);
        [SerializeField]
        private Operation operation;
        [SerializeField]
        private bool operateWhenUpdate;
        public override void Awake()
        {
           InitVariable(randomInt);
        }
        public override void Start() {
            base.Start();
            int random=UnityEngine.Random.Range(range.x,range.y);
            randomInt.Value=(operation==Operation.Absolutely?0:randomInt.Value)+random;
        }
        protected override Status OnUpdate()
        {
            if(!operateWhenUpdate)return Status.Success;
            int random=UnityEngine.Random.Range(range.x,range.y);
            randomInt.Value=(operation==Operation.Absolutely?0:randomInt.Value)+random;
            return Status.Success;
        }
}
}
