using UnityEngine;
using Kurisu.AkiBT;
namespace Kurisu.AkiST
{
    [AkiInfo("Composite:技能序列,除了序列基础功能,技能版本会在序列第一次结束时自动退出")]
    [AkiLabel("Skill:Sequence")]
    [AkiGroup("Skill")]
    public class SkillSequence : Composite
    {
        [SerializeField,AkiLabel("在判断改变时打断子结点")]
        private bool abortOnConditionChanged = true;
        private SkillTreeSO skill;
        private NodeBehavior runningNode;
        protected override void OnAwake() {
            skill=tree as SkillTreeSO;
        }
        public override bool CanUpdate()
        {
            //this node can update when all children can update
            foreach (var child in Children)
            {
                if (!child.CanUpdate())
                {
                    return false;
                }
            }
            return true;
        }

        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                if (abortOnConditionChanged && IsConditionChanged(runningNode))
                {
                    runningNode.Abort();
                    return UpdateWhileSuccess(0);
                }

                var currentOrder = Children.IndexOf(runningNode);
                var status = runningNode.Update();
                if (status == Status.Success)
                {
                    // update next nodes
                    return UpdateWhileSuccess(currentOrder + 1);
                }

                return HandleStatus(status, runningNode);
            }

            return UpdateWhileSuccess(0);

        }

        private bool IsConditionChanged(NodeBehavior runningChild)
        {
            // when the conditions of a node with a higher priority than itself can not update.
            var priority = Children.IndexOf(runningChild);
            for (var i = 0; i < priority; i++)
            {
                var candidate = Children[i];
                if (!candidate.CanUpdate())
                {
                    return true;
                }
            }

            return false;
        }

        private Status UpdateWhileSuccess(int start)
        {
            for (var i = start; i < Children.Count; i++)
            {
                var target = Children[i];
                var childStatus = target.Update();
                if (childStatus == Status.Success)
                {
                    continue;
                }
                return HandleStatus(childStatus, target);
            }
            runningNode = null;
            skill.OnSkillExit();
            return HandleStatus(Status.Success, null);
        }

        private Status HandleStatus(Status status, NodeBehavior updated)
        {
            runningNode = status == Status.Running ? updated : null;
            return status;
        }

        public override void Abort()
        {
            if (runningNode != null)
            {
                runningNode.Abort();
                runningNode = null;
            }
        }
    }
}