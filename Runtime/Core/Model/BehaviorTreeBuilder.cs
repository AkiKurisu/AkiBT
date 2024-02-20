using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace Kurisu.AkiBT
{
    public class BehaviorTreeBuilder : IVariableSource
    {
        private readonly GameObject bindObject;
        private readonly Stack<NodeBehavior> nodeStack;
        private readonly Stack<NodeBehavior> pointers;
        public List<SharedVariable> SharedVariables { get; }
        public BehaviorTreeBuilder(GameObject bindObject)
        {
            this.bindObject = bindObject;
            nodeStack = new();
            SharedVariables = new();
            pointers = new();
        }
        /// <summary>
        /// Begin writing child nodes
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeBuilder BeginChild()
        {
            pointers.Push(nodeStack.Peek());
            return this;
        }
        /// <summary>
        /// Begin writing child nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder BeginChild(NodeBehavior node)
        {
            nodeStack.Push(node);
            pointers.Push(node);
            return this;
        }
        /// <summary>
        /// Add child node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Append(NodeBehavior node)
        {
            nodeStack.Push(node);
            return this;
        }
        /// <summary>
        /// End writing child nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder EndChild(NodeBehavior node)
        {
            nodeStack.Push(node);
            return EndChild();
        }
        /// <summary>
        /// End writing child nodes
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeBuilder EndChild()
        {
            var pointer = pointers.Pop();
            //reverse order
            int childCount = 0;
            while (nodeStack.Peek() != pointer)
            {
                ++childCount;
                pointers.Push(nodeStack.Pop());
            }
            for (int i = 0; i < childCount; ++i)
            {
                var childNode = pointers.Pop();
                if (pointer is Composite composite)
                    composite.AddChild(childNode);
                else if (pointer is Conditional conditional)
                    conditional.Child = childNode;
                else if (pointer is Decorator decorator)
                    decorator.Child = childNode;
            }
            return this;
        }
        /// <summary>
        /// Build behavior tree
        /// </summary>
        /// <param name="behaviorTree"></param>
        /// <returns></returns>
        public bool Build(out BehaviorTree behaviorTree)
        {
            if (pointers.Count > 0)
            {
                Debug.LogWarning("The number of calls to EndChild is not consistent with StartChild");
            }
            pointers.Clear();
            int excess = nodeStack.Count - 1;
            if (excess == -1)
            {
                Debug.LogError("Build failed, no node is added to the builder");
                behaviorTree = null;
                return false;
            }
            if (excess > 0)
            {
                for (int i = 0; i < excess; ++i)
                {
                    nodeStack.Pop();
                }
            }
            behaviorTree = bindObject.AddComponent<BehaviorTree>();
            behaviorTree.SharedVariables.AddRange(SharedVariables);
            behaviorTree.Root = new Root() { Child = nodeStack.Pop() };
            SharedVariables.Clear();
            return true;
        }
        /// <summary>
        /// Get new <see cref="SharedFloat"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedFloat NewFloat(string key, float defaultValue = default)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<float> variable))
            {
                SharedVariables.Add(variable = new SharedFloat()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedFloat;
        }
        /// <summary>
        /// Get new <see cref="SharedInt"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedInt NewInt(string key, int defaultValue = default)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<int> variable))
            {
                SharedVariables.Add(variable = new SharedInt()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedInt;
        }
        /// <summary>
        /// Get new <see cref="SharedVector3"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedVector3 NewVector3(string key, Vector3 defaultValue = default)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<Vector3> variable))
            {
                SharedVariables.Add(variable = new SharedVector3()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedVector3;
        }
        /// <summary>
        /// Get new <see cref="SharedBool"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedBool NewBool(string key, bool defaultValue = default)
        {
            if (!this.TryGetSharedVariable(key, out SharedVariable<bool> variable))
            {
                SharedVariables.Add(variable = new SharedBool()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedBool;
        }
        /// <summary>
        /// Get new <see cref="SharedString"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedString NewString(string key, string defaultValue = "")
        {
            if (!this.TryGetSharedString(key, out SharedVariable<string> variable))
            {
                SharedVariables.Add(variable = new SharedString()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedString;
        }
        /// <summary>
        /// Get new <see cref="SharedTObject<TObject>"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedTObject<TObject> NewObject<TObject>(string key, TObject defaultValue = null) where TObject : UObject
        {
            if (!this.TryGetSharedObject(key, out var variable))
            {
                //Recommend to use sharedObject
                SharedVariables.Add(variable = new SharedObject()
                {
                    Name = key,
                    Value = defaultValue,
                    ConstraintTypeAQN = typeof(TObject).AssemblyQualifiedName,
                    IsShared = true
                });
            }
            return (variable as SharedObject).ConvertT<TObject>();
        }
        /// <summary>
        /// Get new <see cref="SharedObject"/>, also write it to the public variables of the behavior tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public SharedObject NewObject(string key, UObject defaultValue = null)
        {
            if (!this.TryGetSharedObject(key, out SharedVariable<UObject> variable))
            {
                SharedVariables.Add(variable = new SharedObject()
                {
                    Name = key,
                    Value = defaultValue,
                    IsShared = true
                });
            }
            return variable.Clone() as SharedObject;
        }
    }
}
