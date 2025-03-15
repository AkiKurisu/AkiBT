using System;
using System.Collections.Generic;
using UnityEngine.Pool;
namespace Kurisu.AkiBT.DSL
{
    public sealed class BuildParserListener : IParserListener, IDisposable
    {
        private static readonly ObjectPool<BuildParserListener> Pool = new(() => new BuildParserListener());
        
        private BuildVisitor _visitor;
        
        private readonly List<SharedVariable> _variables = new();
        
        private readonly List<NodeBehavior> _nodes = new();
        
        public BuildParserListener Verbose(bool verbose)
        {
            _visitor.Verbose = verbose;
            return this;
        }
        
        public void PushTopLevelExpression(NodeExprAST data)
        {
            _visitor.VisitNodeExprAST(data);
            _nodes.Add(_visitor.NodeStack.Pop());
        }
        
        public void PushVariableDefinition(VariableDefineExprAST data)
        {
            _visitor.VisitVariableDefineAST(data);
            _variables.Add(_visitor.VariableStack.Pop());
        }
        
        /// <summary>
        /// Build behavior tree
        /// </summary>
        /// <returns></returns>
        public BehaviorTree Build()
        {
            var sequence = new Sequence();
            foreach (var node in _nodes)
                sequence.AddChild(node);
            var instance = new BehaviorTree
            {
                variables = new List<SharedVariable>(_variables),
                root = new Root { Child = sequence }
            };
            _variables.Clear();
            _nodes.Clear();
            return instance;
        }
        
        /// <summary>
        /// Get pooled build listener
        /// </summary>
        /// <param name="buildVisitor">The build visitor attached to</param>
        /// <returns></returns>
        public static BuildParserListener GetPooled(BuildVisitor buildVisitor)
        {
            var listener = Pool.Get();
            listener._visitor = buildVisitor;
            return listener;
        }
        
        public void Dispose()
        {
            _visitor?.Dispose();
            _visitor = null;
            _variables.Clear();
            _nodes.Clear();
            Pool.Release(this);
        }
    }
}