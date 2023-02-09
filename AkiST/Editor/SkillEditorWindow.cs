using System;
using Kurisu.AkiBT;
using Kurisu.AkiBT.Editor;
namespace Kurisu.AkiST.Editor
{
public class SkillEditorWindow : GraphEditorWindow
{
        protected override string TreeName=>"技能树";
        protected override string InfoText=>"欢迎使用AkiST,一个为技能系统定制的行为树!";
        public static new void Show(IBehaviorTree bt)
        {
            var window = Create<SkillEditorWindow>(bt);
            window.Show();
            window.Focus();
        } 
        protected override BehaviorTreeView CreateView(IBehaviorTree behaviorTree)
        {
            return new SkillTreeView(behaviorTree, this);
        }
}
}