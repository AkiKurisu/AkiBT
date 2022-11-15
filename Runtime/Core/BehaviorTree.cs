using UnityEngine;
namespace Kurisu.AkiBT
{
    public enum UpdateType
    {
        [InspectorName("自动模式")]
       Auto,
       [InspectorName("手动模式")]
       Manual
    }
    [DisallowMultipleComponent]
    /// <summary>
    /// 行为树
    /// </summary>
    public class BehaviorTree : MonoBehaviour
    {
        
        [HideInInspector]
        [SerializeReference]
        private Root root = new Root();
        

        [SerializeField,
        Tooltip("切换成UpdateType.Manual使用手动更新并且调用BehaviorTree.Tick()"),Header("Kurisu AkiBT行为树")]
        private UpdateType updateType;
        [SerializeField,
        Tooltip("使用外部行为树替换组件内行为树,保存时会覆盖组件内行为树")]
        private BehaviorTreeSO externalBehaviorTree;
        public BehaviorTreeSO ExternalBehaviorTree=>externalBehaviorTree;
        [SerializeField,HideInInspector]
        private bool autoSave;
        [SerializeField,HideInInspector]
        private string savePath="Assets";
        public string SavePath
        {
             get => savePath;
        #if UNITY_EDITOR
                    set => savePath = value;
        #endif
        }
        public bool AutoSave
        {

            get => autoSave;
#if UNITY_EDITOR
            set => autoSave = value;
#endif
        }
        
        public Root Root
        {
            get => root;
#if UNITY_EDITOR
            set => root = value;
#endif
        }
        
        private void Awake() {
            root.Run(gameObject);
            root.Awake();
        }

        private void Start()
        {
            root.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }
        
        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }

    }
}