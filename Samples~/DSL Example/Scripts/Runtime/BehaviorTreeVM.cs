using UnityEngine;
using Kurisu.AkiBT.DSL;
using System.IO;

namespace Kurisu.AkiBT.Example
{
    public class BehaviorTreeVM : MonoBehaviour
    {
        private Compiler _compiler;
        
        private BehaviorTree _behaviorTree;

        private BehaviorTreeComponent _behaviorTreeComponent;

        private bool _isUpdating;

        private void Start()
        {
            _behaviorTreeComponent = gameObject.AddComponent<BehaviorTreeComponent>();
            _behaviorTreeComponent.updateType = BehaviorTreeComponent.UpdateType.Manual;
            _compiler = new Compiler(Path.Combine(Application.streamingAssetsPath, "NodeTypeRegistry.json"));
        }

        private void Update()
        {
            if (_isUpdating)
            {
                _behaviorTreeComponent.Tick();
            }
        }

        private void OnDestroy()
        {
            _behaviorTree?.Dispose();
            _behaviorTree = null;
        }
        
        public void Compile(string dsl)
        {
            Stop();
            Dispose();
            _behaviorTree = _compiler.Verbose(true).Compile(dsl);
            Run();
        }
        
        public void Dispose()
        {
            _behaviorTree?.Dispose();
            _behaviorTree = null;
        }
        
        public void Run()
        {
            _isUpdating = true;
            _behaviorTreeComponent.SetRuntimeBehaviorTree(_behaviorTree);
        }
        
        public void Stop()
        {
            _isUpdating = false;
        }
    }
}
