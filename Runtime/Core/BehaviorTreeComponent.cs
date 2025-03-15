using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kurisu.AkiBT
{
    /// <summary>
    /// Behavior Tree Component
    /// Awake, Start and Update using UnityEngine's life cycle
    /// </summary>
    [DisallowMultipleComponent]
    public class BehaviorTreeComponent : MonoBehaviour, IBehaviorTreeContainer
    {
        public enum UpdateType
        {
            Auto,
            Manual
        }
        
        Object IBehaviorTreeContainer.Object => gameObject;
        
        [Tooltip("Switch to UpdateType.Manual to use manual updates and call BehaviorTree.Tick()")]
        public UpdateType updateType;
        
        [SerializeField, Tooltip("Use the external behavior tree to replace the behavior tree in the component," +
                                 " and the behavior tree in the component will be overwritten when saving")]
        private BehaviorTreeAsset externalBehaviorTree;
        
        [NonSerialized]
        private BehaviorTree _instance;
        
        [SerializeField, HideInInspector]
        
        private BehaviorTreeData behaviorTreeData = new();
        
        private BlackBoardComponent _blackBoardCmp;
        
        private BlackBoard _blackBoard;
        
        private void Awake()
        {
            _blackBoardCmp = GetComponent<BlackBoardComponent>();
            if (_blackBoardCmp) _blackBoard = _blackBoardCmp.GetBlackBoard();
            // Assign instance only at runtime
            InitBehaviorTree(_instance = GetBehaviorTree());
            _instance.Awake();
        }
        
        private void Start()
        {
            _instance.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto)
            {
                Tick();
            }
        }
        
        private void OnDestroy()
        {
            _instance.Dispose();
            _instance = null;
        }
        
        public void Tick()
        {
            _instance.Tick();
        }
        
        private void InitBehaviorTree(BehaviorTree behaviorTreeInstance)
        {
            // Initialize before run
            behaviorTreeInstance.InitVariables();
            if (_blackBoard != null)
            {
                // Need to ensure the order of the mapping chain
                behaviorTreeInstance.BlackBoard.MapTo(_blackBoard);
                // chain: node => tree => blackBoard => global variables
                _blackBoard.MapGlobal();
            }
            else
            {
                behaviorTreeInstance.BlackBoard.MapGlobal();
            }
            behaviorTreeInstance.Run(gameObject);
        }
        
        public BehaviorTree GetBehaviorTree()
        {
            if (Application.isPlaying && _instance != null)
            {
                return _instance;
            }
            return externalBehaviorTree ? externalBehaviorTree.GetBehaviorTree() : behaviorTreeData.CreateInstance();
        }
        
        public void SetBehaviorTreeData(BehaviorTreeData serializedData)
        {
            behaviorTreeData = serializedData;
        }
        
        public void SetRuntimeBehaviorTree(BehaviorTree behaviorTree)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Can not assign behavior tree instance in Editor mode");
                return;
            }
            _instance?.Abort();
            _instance?.Dispose();
            InitBehaviorTree(_instance = behaviorTree);
            _instance.Awake();
            _instance.Start();
        }
    }
}