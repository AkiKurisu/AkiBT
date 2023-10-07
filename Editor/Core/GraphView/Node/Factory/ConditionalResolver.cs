using System;
namespace Kurisu.AkiBT.Editor
{
    public class ConditionalResolver : INodeResolver
    {
        public IBehaviorTreeNode CreateNodeInstance(Type type)
        {
            return new ConditionalNode();
        }
        public static bool IsAcceptable(Type behaviorType) => behaviorType.IsSubclassOf(typeof(Conditional));
    }
}
