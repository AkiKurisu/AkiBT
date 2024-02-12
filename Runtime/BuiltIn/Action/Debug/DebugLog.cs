using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action: Log some text")]
    [AkiLabel("Debug: Log")]
    [AkiGroup("Debug")]
    public class DebugLog : Action
    {
        public SharedString logText;
        protected override Status OnUpdate()
        {
            Debug.Log(logText.Value, GameObject);
            return Status.Success;
        }
    }
}
