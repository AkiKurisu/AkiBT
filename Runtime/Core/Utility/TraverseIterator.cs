using System.Collections.Generic;
namespace Kurisu.AkiBT
{
    public readonly struct TraverseIterator
    {
        private static readonly Stack<NodeBehavior> stack = new();
        private static readonly object iteratorLock = new();
        private readonly IBehaviorTree behaviorTree;
        public TraverseIterator(IBehaviorTree behaviorTree)
        {
            this.behaviorTree = behaviorTree;
        }
        public IEnumerator<NodeBehavior> GetEnumerator()
        {
            lock (iteratorLock)
            {
                stack.Clear();
                stack.Push(behaviorTree.Root);
                while (stack.Count != 0)
                {
                    var top = stack.Pop();
                    yield return top;
                    if (top is not IIterable iterable) continue;
                    for (int i = iterable.GetChildCount() - 1; i >= 0; --i)
                    {
                        stack.Push(iterable.GetChildAt(i));
                    }
                }
            }
        }
    }
}
