using System;
namespace Kurisu.AkiBT.Editor
{
    public class DecoratorResolver : INodeResolver
    {
        public IBehaviorTreeNode CreateNodeInstance(Type type)
        {
            return new DecoratorNode();
        }
        public static bool IsAcceptable(Type behaviorType) => behaviorType.IsSubclassOf(typeof(Decorator));
    }
}
