using UObject = UnityEngine.Object;
namespace Kurisu.AkiBT
{
        public interface IBehaviorTreeContainer
        {
                /// <summary>
                /// Container referenced <see cref="UnityEngine.Object"/>
                /// </summary>
                /// <value></value>
                UObject Object { get; }
                /// <summary>
                /// Get behavior tree instance
                /// </summary>
                /// <returns></returns>
                BehaviorTree GetBehaviorTree();
                /// <summary>
                /// Set behavior tree data, used for persistent data
                /// </summary>
                /// <param name="behaviorTreeData"></param>
                void SetBehaviorTreeData(BehaviorTreeData behaviorTreeData);
        }
}