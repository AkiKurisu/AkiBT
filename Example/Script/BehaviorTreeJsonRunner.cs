using UnityEngine;
namespace Kurisu.AkiBT.Example
{
    public class BehaviorTreeJsonRunner : MonoBehaviour
    {
        [SerializeField]
        private TextAsset serializedData;
        private BehaviorTreeSO behaviorTreeSO;
        private void Awake()
        {
            behaviorTreeSO = ScriptableObject.CreateInstance<BehaviorTreeSO>();
            behaviorTreeSO.Deserialize(serializedData.text);
            if (!behaviorTreeSO.IsInitialized) behaviorTreeSO.Initialize();
            behaviorTreeSO.Init(gameObject);
        }
        private void Update()
        {
            behaviorTreeSO.Update();
        }
    }
}
