using UnityEngine;
using Kurisu.AkiBT.DSL;
using System.IO;
namespace Kurisu.AkiBT.Example
{
    public class BehaviorTreeVM : MonoBehaviour
    {
        private Compiler compiler;
        [SerializeField]
        private bool isPlaying;
        public bool IsPlaying => isPlaying;
        [SerializeField]
        private BehaviorTreeSO behaviorTreeSO;
        public BehaviorTreeSO BehaviorTreeSO => behaviorTreeSO;
        private void Start()
        {
            compiler = new Compiler(Path.Combine(Application.streamingAssetsPath, "NodeTypeRegistry.json"));
        }
        /// <summary>
        /// Compile vmCode. if success, you will have a temporary BehaviorTreeSO inside this component.
        /// You can save this BehaviorTreeSO in the editor by clicking 'Save' button
        /// </summary>
        /// <param name="vmCode"></param>
        public void Compile(string vmCode)
        {
            if (behaviorTreeSO != null) Clear();
            behaviorTreeSO = compiler.Verbose(true).Compile(vmCode);
        }
        public void Clear()
        {
            behaviorTreeSO = null;
            isPlaying = false;
        }
        /// <summary>
        /// VMBehaviorTreeSO doesn't automatically awake and start, you need to run it manually
        /// </summary>
        public void Run()
        {
            if (behaviorTreeSO == null || isPlaying) return;
            behaviorTreeSO.Initialize();
            behaviorTreeSO.Init(gameObject);
            isPlaying = true;
        }
        public void Stop()
        {
            isPlaying = false;
        }
        private void Update()
        {
            if (isPlaying) behaviorTreeSO.Update();
        }

    }
}
