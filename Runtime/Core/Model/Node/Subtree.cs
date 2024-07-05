using UnityEngine;
using UnityEngine.Assertions;
namespace Kurisu.AkiBT
{
    [AkiInfo("Subtree: An embedding subtree instance.")]
    [AkiGroup("Hidden")]
    public class Subtree : Action, IBehaviorTreeContainer
    {
        private BehaviorTree instance;
        // should not use shared mode because it can not guarantee instance initialization
        [HideInEditorWindow]
        public BehaviorTreeAsset subtree;

        public Object Object => subtree;

        public override void Abort()
        {
            if (subtree == null) return;
            instance.Abort();
        }
        public override void Awake()
        {
            if (subtree == null) return;
            instance = subtree.GetBehaviorTree();
            Assert.IsNotNull(instance.root);
            // inherit variables if possible
            instance.MapTo(Tree);
            instance.InitVariables();
            instance.Awake();
        }
        public override void Start()
        {
            if (subtree == null) return;
            instance.Start();
        }
        protected override Status OnUpdate()
        {
            if (subtree == null) return Status.Success;
            return instance.TickWithStatus();
        }

        public BehaviorTree GetBehaviorTree()
        {
            if (Application.isPlaying) return instance;
            return subtree.GetBehaviorTree();
        }

        public void SetBehaviorTreeData(BehaviorTreeData behaviorTreeData)
        {
            if (!Application.isPlaying || !subtree) return;
            subtree.SetBehaviorTreeData(behaviorTreeData);
        }
    }
}
