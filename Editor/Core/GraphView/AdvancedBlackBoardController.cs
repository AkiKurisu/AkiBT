using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    internal class AdvancedBlackBoardController : IVariableSource
    {
        private class VirtualGraphView : GraphView { }
        private readonly AdvancedBlackBoard blackBoard;
        public List<SharedVariable> SharedVariables { get; } = new();
        private readonly IVariableSource source;
        private bool isDirty;
        private readonly System.Action onUpdate;
        public AdvancedBlackBoardController(IVariableSource source, AdvancedBlackBoard.BlackBoardSettings settings = default, System.Action onUpdate = null)
        {
            this.source = source;
            this.onUpdate = onUpdate;
            //Need attached to a virtual graphView to send event
            //It's an interesting hack so that you can use blackBoard outside of graphView
            blackBoard = new AdvancedBlackBoard(this, new VirtualGraphView(), settings);
            blackBoard.style.position = Position.Relative;
            blackBoard.style.width = Length.Percent(100f);
            foreach (var variable in source.SharedVariables)
            {
                if (Application.isPlaying)
                {
                    blackBoard.AddSharedVariable(variable);
                }
                else
                {
                    blackBoard.AddSharedVariable(variable.Clone());
                }
            }
            blackBoard.RegisterCallback<VariableChangeEvent>(_ => isDirty = true);
        }
        public AdvancedBlackBoard GetBlackBoard() => blackBoard;
        public void Update()
        {
            source.SharedVariables.Clear();
            source.SharedVariables.AddRange(SharedVariables);
            onUpdate?.Invoke();
        }
        public void UpdateIfDirty()
        {
            if (isDirty)
            {
                Update();
                isDirty = false;
            }
        }
    }
}