using System.Collections.Generic;
using UnityEditor;
using Kurisu.AkiBT;
using Kurisu.AkiBT.Editor;
namespace Kurisu.AkiST.Editor
{
    public class SkillTreeView : BehaviorTreeView
    {
        /// <summary>
        /// 添加显示的结点Group,如果不需要筛选可以置为Null
        /// </summary>
        /// <value></value>
        static readonly string[] showGroupName={"Skill","Math"};
        public SkillTreeView(IBehaviorTree bt, EditorWindow editor) : base(bt, editor)
        {
            provider.SetShowGroupNames(showGroupName);
        }
        protected override string treeEditorName=>"AkiST";
        protected override bool Validate()
        {
            var stack = new Stack<BehaviorTreeNode>();
            bool findExit=false;
            stack.Push(root);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!node.Validate(stack))
                {
                    return false;
                }
                //简单检测是否有技能出口
                if(node.GetBehavior().Equals(typeof(SkillExit))||node.GetBehavior().Equals(typeof(SkillSequence)))findExit=true;
            }
            if(!findExit)return false;
            return true;
        }
    }
}