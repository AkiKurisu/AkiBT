using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public interface INodeResolver
    {
        IBehaviorTreeNode CreateNodeInstance(Type type);
    }
    public class NodeResolverFactory
    {
        private static NodeResolverFactory instance;
        public static NodeResolverFactory Instance => instance ?? new NodeResolverFactory();
        private StyleSheet styleSheetCache;
        private readonly List<Type> _ResolverTypes = new();
        public NodeResolverFactory()
        {
            instance = this;
            _ResolverTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(x => x.GetTypes())
            .SelectMany(x => x)
            .Where(x => IsValidType(x))
            .ToList();
            _ResolverTypes.Sort((a, b) =>
            {
                var aOrdered = a.GetCustomAttribute<Ordered>(false);
                var bOrdered = b.GetCustomAttribute<Ordered>(false);
                if (aOrdered == null && bOrdered == null) return 0;
                if (aOrdered != null && bOrdered != null) return aOrdered.Order - bOrdered.Order;
                if (aOrdered != null) return -1;
                return 1;
            });
        }
        private static bool IsValidType(Type type)
        {
            if (type.IsAbstract) return false;
            if (type.GetMethod("IsAcceptable") == null) return false;
            if (!type.GetInterfaces().Any(t => t == typeof(INodeResolver))) return false;
            return true;
        }
        public IBehaviorTreeNode Create(Type behaviorType, ITreeView treeView)
        {
            IBehaviorTreeNode node = null;
            bool find = false;
            foreach (var _type in _ResolverTypes)
            {
                if (!IsAcceptable(_type, behaviorType)) continue;
                node = (Activator.CreateInstance(_type) as INodeResolver).CreateNodeInstance(behaviorType);
                find = true;
                break;
            }
            if (!find) node = new ActionNode();
            node.SetBehavior(behaviorType, treeView);
            if (styleSheetCache == null) styleSheetCache = BehaviorTreeSetting.GetNodeStyle(treeView.TreeEditorName);
            node.View.styleSheets.Add(styleSheetCache);
            return node;
        }
        private static bool IsAcceptable(Type type, Type behaviorType)
        {
            return (bool)type.InvokeMember("IsAcceptable", BindingFlags.InvokeMethod, null, null, new object[] { behaviorType });
        }
    }
}