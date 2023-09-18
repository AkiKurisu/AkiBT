using System;
namespace Kurisu.AkiBT.Editor
{
    public class CompositeResolver : INodeResolver
    {
        public IBehaviorTreeNode CreateNodeInstance(Type type)
        {
            return new CompositeNode();
        }
        public static bool IsAcceptable(Type behaviorType) => behaviorType.IsSubclassOf(typeof(Composite));
    }
}
