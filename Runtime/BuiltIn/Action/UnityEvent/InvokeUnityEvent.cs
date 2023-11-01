using UnityEngine;
using UnityEngine.Events;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action : Invoke a UnityEvent, should be noticed that it may loss reference during serialization ")]
    [AkiLabel("UnityEvent : Invoke UnityEvent")]
    [AkiGroup("UnityEvent")]
    public class InvokeUnityEvent : Action
    {
        [SerializeField, WrapField]
        private UnityEvent unityEvent;
        protected override Status OnUpdate()
        {
            unityEvent?.Invoke();
            return Status.Success;
        }
    }
}
