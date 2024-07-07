using UnityEngine;
namespace Kurisu.AkiBT
{
    public enum UpdateType
    {
        Auto,
        Manual
    }
    /// <summary>
    /// Behavior Tree Component
    /// Awake, Start and Update using UnityEngine's life cycle
    /// </summary>
    [DisallowMultipleComponent]
    public class BehaviorTreeComponent : MonoBehaviour, IBehaviorTreeContainer
    {
        Object IBehaviorTreeContainer.Object => gameObject;
        [SerializeField,
        Tooltip("Switch to UpdateType.Manual to use manual updates and call BehaviorTree.Tick()")]
        private UpdateType updateType;
        [SerializeField, Tooltip("Use the external behavior tree to replace the behavior tree in the component," +
                                 " and the behavior tree in the component will be overwritten when saving")]
        private BehaviorTreeAsset externalBehaviorTree;
        private BehaviorTree instance = null;
        [SerializeField, HideInInspector]
        private BehaviorTreeData behaviorTreeData = new();
        private BlackBoardComponent blackBoardCmp;
        private BlackBoard blackBoard;
        private void Awake()
        {
            blackBoardCmp = GetComponent<BlackBoardComponent>();
            if (blackBoardCmp) blackBoard = blackBoardCmp.GetBlackBoard();
            // Assign instance only at runtime
            InitBehaviorTree(instance = GetBehaviorTree());
            instance.Awake();
        }
        private void Start()
        {
            instance.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }

        public void Tick()
        {
            instance.Tick();
        }
        private void InitBehaviorTree(BehaviorTree instance)
        {
            // Initialize before run
            instance.InitVariables();
            if (blackBoard != null)
            {
                // Need to ensure the order of the mapping chain
                instance.BlackBoard.MapTo(blackBoard);
                // chain: node => tree => blackBoard => global variables
                blackBoard.MapGlobal();
            }
            else
            {
                instance.BlackBoard.MapGlobal();
            }
            instance.Run(gameObject);
        }
        public BehaviorTree GetBehaviorTree()
        {
            if (Application.isPlaying && instance != null)
            {
                return instance;
            }
            if (externalBehaviorTree)
            {
                return externalBehaviorTree.GetBehaviorTree();
            }
            else
            {
                return behaviorTreeData.CreateInstance();
            }
        }
        public void SetBehaviorTreeData(BehaviorTreeData behaviorTreeData)
        {
            this.behaviorTreeData = behaviorTreeData;
        }
        public void SetRuntimeBehaviorTree(BehaviorTree behaviorTree)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can not assign behavior tree instance in Editor mode");
                return;
            }
            instance?.Abort();
            InitBehaviorTree(instance = behaviorTree);
            instance.Awake();
            instance.Start();
        }
    }
}