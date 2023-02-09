using System.Collections.Generic;
using UnityEditor;
using Kurisu.AkiBT;
using Kurisu.AkiBT.Editor;
namespace Kurisu.AkiST.Editor
{
    public class SkillTreeView : BehaviorTreeView
    {
        public SkillTreeView(IBehaviorTree bt, EditorWindow editor) : base(bt, editor)
        {
        }

        public override sealed string treeEditorName=>"AkiST";
        protected sealed override bool Validate()
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
            return findExit;
        }
    }
}