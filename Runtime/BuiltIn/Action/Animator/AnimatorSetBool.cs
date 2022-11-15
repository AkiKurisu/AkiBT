using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:根据status设置Animator的Bool元素")]
    [AkiLabel("Animator:SetBool")]
    [AkiGroup("Animator")]
    public class AnimatorSetBool : AnimatorAction
    {
        [SerializeField]
        private string parameter;
        

        [SerializeField]
        private bool status;
        private int parameterHash;
        public override void Start() {
            parameterHash=Animator.StringToHash(parameter);
        }
        protected override Status OnUpdate()
        {
            animator.SetBool(parameterHash,status);
            return Status.Success;
        }
    }
}