using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
namespace Kurisu.AkiBT
{
    public struct TraverseIterator : IEnumerator<NodeBehavior>
    {
        private readonly Stack<NodeBehavior> stack;
        private static readonly ObjectPool<Stack<NodeBehavior>> pool = new(() => new(), null, s => s.Clear());
        private NodeBehavior currentNode;
        public TraverseIterator(NodeBehavior root)
        {
            stack = pool.Get();
            currentNode = null;
            if (root != null)
            {
                stack.Push(root);
            }
        }

        public readonly NodeBehavior Current
        {
            get
            {
                if (currentNode == null)
                {
                    throw new InvalidOperationException();
                }
                return currentNode;
            }
        }

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            pool.Release(stack);
            currentNode = null;
        }
        public bool MoveNext()
        {
            if (stack.Count == 0)
            {
                return false;
            }

            currentNode = stack.Pop();
            int childrenCount = currentNode.GetChildrenCount();
            for (int i = childrenCount - 1; i >= 0; i--)
            {
                stack.Push(currentNode.GetChildAt(i));
            }
            return true;
        }
        public void Reset()
        {
            stack.Clear();
            if (currentNode != null)
            {
                stack.Push(currentNode);
            }
            currentNode = null;
        }
    }
}
