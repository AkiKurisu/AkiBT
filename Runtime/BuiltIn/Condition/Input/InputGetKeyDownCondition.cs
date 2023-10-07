using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition : Returns Status.Success when the specified key is pressed, otherwise returns Status.Failure")]
    [AkiLabel("Input : GetKeyDown")]
    [AkiGroup("Input")]
    public class InputGetKeyDownCondition : Conditional
    {
        [SerializeField]
        private KeyCode keyToGet;
        protected override bool IsUpdatable()
        {
            return Input.GetKeyDown(keyToGet);
        }
    }
}
