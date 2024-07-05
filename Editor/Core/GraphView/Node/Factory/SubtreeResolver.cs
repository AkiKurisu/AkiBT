using System;
namespace Kurisu.AkiBT.Editor
{
    public class SubtreeResolver : INodeResolver
    {
        public IBehaviorTreeNode CreateNodeInstance(Type type)
        {
            return new SubtreeNode();
        }
        public static bool IsAcceptable(Type behaviorType) => behaviorType == typeof(Subtree);
    }
}
