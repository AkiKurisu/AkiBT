using UnityEngine;
namespace Kurisu.AkiBT.Example
{
    /// <summary>
    /// Update behavior tree so directly sample
    /// However you should know scriptableObject is an asset
    /// If you want to run multi behavior tree, should use <see cref="BehaviorTree"/> to create instance
    /// or use json to deserialize new <see cref="BehaviorTreeJsonRunner"/>
    /// </summary>
    public class BehaviorTreeSORunner : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTreeSO behaviorTreeSO;
        private void Awake()
        {
            behaviorTreeSO.Init(gameObject);
        }
        private void Update()
        {
            behaviorTreeSO.Update();
        }
    }
}
