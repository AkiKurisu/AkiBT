using UnityEngine;
namespace Kurisu.AkiBT.Extend
{
    [AkiInfo("Action:获取Animator的Bool元素,如果和status一致返回True,否则返回False")]
    [AkiLabel("Animator:GetBool")]
    [AkiGroup("Animator")]
    public class AnimatorGetBool : AnimatorCondition
    {
        [SerializeField]
        private string parameter;
        [SerializeField]
        private bool status;
        private int parameterHash;
        protected override void OnStart()
        {
            base.OnStart();
            parameterHash=Animator.StringToHash(parameter);
        }
        protected override bool IsUpdatable()
        {
            return(animator.GetBool(parameterHash)==status);
        }
    }
}