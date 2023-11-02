using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kurisu.AkiBT
{
    /// <summary>
    /// Global variables are variables managed by a variable scope and any behavior tree initialized in this scope
    /// will map global variable <see cref="SharedVariable.IsGlobal"/> to it
    /// </summary>
    public class GlobalVariables : IVariableSource, IDisposable
    {
        public List<SharedVariable> SharedVariables { get; }
        private static GlobalVariables instance;
        public static GlobalVariables Instance => instance ?? FindOrCreateDefault();
        private readonly IVariableScope parentScope;
        public GlobalVariables(List<SharedVariable> sharedVariables)
        {
            instance = this;
            SharedVariables = new List<SharedVariable>(sharedVariables);
        }
        public GlobalVariables(List<SharedVariable> sharedVariables, IVariableScope parentScope)
        {
            instance = this;
            this.parentScope = parentScope;
            SharedVariables = new List<SharedVariable>(sharedVariables);
            if (parentScope != null)
            {
                sharedVariables.AddRange(parentScope.GlobalVariables.SharedVariables);
            }
        }
        private static GlobalVariables FindOrCreateDefault()
        {
            var scope = Object.FindObjectOfType<SceneVariableScope>();
            if (scope != null)
            {
                scope.Initialize();
                return scope.GlobalVariables;
            }
            instance = new(new());
            return instance;
        }
        public void Dispose()
        {
            if (instance != this)
            {
                Debug.LogWarning("Only scope current used should be disposed!");
                return;
            }
            instance = null;
            if (parentScope != null)
            {
                instance = parentScope.GlobalVariables;
            }
        }
    }
}