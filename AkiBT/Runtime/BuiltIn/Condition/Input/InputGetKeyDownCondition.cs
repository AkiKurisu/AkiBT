using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Condition:当指定按键被按下时返回True,否则返回False")]
    [AkiLabel("Input:GetKeyDown")]
    [AkiGroup("Input")]
    public class InputGetKeyDownCondition : Conditional
    {
        [SerializeField]
        private KeyCode keyToGet;
        protected override bool IsUpdatable()
        {
            return(Input.GetKeyDown(keyToGet));
        }
    }
}
