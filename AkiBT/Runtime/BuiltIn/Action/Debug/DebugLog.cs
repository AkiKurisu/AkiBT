using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:DebugLog一段文字")]
    [AkiLabel("Debug:Log")]
    [AkiGroup("Debug")]
    public class DebugLog : Action
    {
        [SerializeField]
        private string logText;
        protected override Status OnUpdate()
        {
            Debug.Log(logText);
            return Status.Success;
        }
    }
}
