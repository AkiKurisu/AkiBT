using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    public readonly struct TraverseIterator
    {
        private static readonly Stack<NodeBehavior> stack = new();
        private static readonly object iteratorLock = new();
        private readonly NodeBehavior iterateRoot;
        private readonly bool includeChildren;
        public TraverseIterator(NodeBehavior iterateRoot, bool includeChildren)
        {
            this.iterateRoot = iterateRoot;
            this.includeChildren = includeChildren;
        }
        public IEnumerator<NodeBehavior> GetEnumerator()
        {
            lock (iteratorLock)
            {
                int depth = 0;
                stack.Clear();
                stack.Push(iterateRoot);
                while (stack.Count != 0)
                {
                    var top = stack.Pop();
                    yield return top;
                    if ((!includeChildren && depth > 0) || top is not IIterable iterable) continue;
                    ++depth;
                    for (int i = iterable.GetChildCount() - 1; i >= 0; --i)
                    {
                        stack.Push(iterable.GetChildAt(i));
                    }
                }
            }
        }
    }
}
