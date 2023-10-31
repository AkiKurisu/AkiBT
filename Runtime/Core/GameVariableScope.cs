using System.Collections.Generic;
using UnityEngine;
namespace Kurisu.AkiBT
{
    [CreateAssetMenu(fileName = "GameVariableScope", menuName = "AkiBT/GameVariableScope")]
    public class GameVariableScope : ScriptableObject, IVariableScope, IVariableSource
    {
        private static readonly Stack<GameVariableScope> initializationStack = new();
        [SerializeReference]
        private List<SharedVariable> sharedVariables = new();
        public List<SharedVariable> SharedVariables => sharedVariables;
        [SerializeField]
        private GameVariableScope parentScope;
        public GlobalVariables GlobalVariables { get; private set; }
        private bool initialized = false;
        private void Awake()
        {
            if (!initialized)
            {
                Initialize();
            }
        }
        public void Initialize()
        {
            if (initializationStack.Contains(this))
            {
                Debug.LogError("Circulating initialization occurs!");
                return;
            }
            initialized = true;
            initializationStack.Push(this);
            if (parentScope && parentScope.IsCurrentScope())
            {
                GlobalVariables = new GlobalVariables(sharedVariables, parentScope);
            }
            else
            {
                GlobalVariables = new GlobalVariables(sharedVariables);
            }
            initializationStack.TryPop(out _);
        }
        public bool IsCurrentScope()
        {
            if (!initialized) Initialize();
            return GlobalVariables.Instance == GlobalVariables;
        }
        private void OnDestroy()
        {
            GlobalVariables.Dispose();
        }
    }
}