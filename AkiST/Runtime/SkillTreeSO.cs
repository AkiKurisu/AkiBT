using UnityEngine;
using Kurisu.AkiBT;
namespace Kurisu.AkiST
{
public abstract class SkillTreeSO : BehaviorTreeSO
{
    public virtual bool UseTree=>true;
    #if UNITY_EDITOR
    [SerializeField,HideInInspector]
    private SkillTreeSO externalSkill;
    public sealed override BehaviorTreeSO ExternalBehaviorTree=>externalSkill;
    # endif
    [Multiline,SerializeField,AkiLabel("技能描述")]
    protected string description;
    public string Description=>description;
    public event System.Action OnSkillExitEvent;
    /// <summary>
    /// 技能退出
    /// </summary>
    public void OnSkillExit()
    {
        OnSkillExitEvent?.Invoke();
    }
}
}