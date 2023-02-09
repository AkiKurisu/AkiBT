using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:Log一段文字")]
    [AkiLabel("Debug:Log")]
    [AkiGroup("Debug")]
    public class DebugLog : Action
    {
        [SerializeField]
        private SharedString logText;
        public override void Awake() {
            InitVariable(logText);
        }
        protected override Status OnUpdate()
        {
            Debug.Log(logText.Value,gameObject);
            return Status.Success;
        }
    }
}
