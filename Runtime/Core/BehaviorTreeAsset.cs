using UnityEngine;
namespace Kurisu.AkiBT
{
        /// <summary>
        /// Behavior tree asset version data holder.
        /// </summary>
        [CreateAssetMenu(fileName = "BehaviorTreeAsset", menuName = "AkiBT/BehaviorTreeAsset")]
        public class BehaviorTreeAsset : ScriptableObject, IBehaviorTreeContainer
        {
#if UNITY_EDITOR
                [SerializeField, Multiline]
                private string description;
#endif
                Object IBehaviorTreeContainer.Object => this;
                [SerializeField, HideInInspector]
                private BehaviorTreeData behaviorTreeData = new();
                public BehaviorTree GetBehaviorTree()
                {
                        // Always return a new instance
                        return behaviorTreeData.CreateInstance();
                }
                public void SetBehaviorTreeData(BehaviorTreeData behaviorTreeData)
                {
                        this.behaviorTreeData = behaviorTreeData;
                }
        }
}