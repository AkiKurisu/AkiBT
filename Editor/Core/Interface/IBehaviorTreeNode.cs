using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public interface IBehaviorTreeNode
    {
        Node View { get; }
        string Guid { get; }
        Port Parent { get; }
        ITreeView MapTreeView { get; }
        Action<IBehaviorTreeNode> OnSelectAction { get; set; }
        void Restore(NodeBehavior behavior);
        void Commit(Stack<IBehaviorTreeNode> stack);
        bool Validate(Stack<IBehaviorTreeNode> stack);
        Type GetBehavior();
        void SetBehavior(Type nodeBehavior, ITreeView ownerTreeView = null);
        void CopyFrom(IBehaviorTreeNode copyNode);
        NodeBehavior ReplaceBehavior();
        void ClearStyle();
        IFieldResolver GetFieldResolver(string fieldName);
        Rect GetWorldPosition();
    }
}