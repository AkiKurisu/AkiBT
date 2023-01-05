using Kurisu.AkiBT;
namespace Kurisu.AkiST
{
    [AkiInfo("Action:退出技能,运行中的技能树需要该结点或相同功能的结点才能正常退出")]
    [AkiLabel("Skill:Exit")]
    [AkiGroup("Skill")]
    public class SkillExit : Action
    {
        protected SkillTreeSO skill;
        public override void Awake() {
            skill=tree as SkillTreeSO;
        }
        protected override Status OnUpdate()
        {
            skill.OnSkillExit();
            return  Status.Success;
        }
    }
}