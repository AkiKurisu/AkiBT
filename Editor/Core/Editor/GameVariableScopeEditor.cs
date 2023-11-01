using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    internal class VirtualGraphView : GraphView { }
    internal class VariableSourceProxy : IVariableSource
    {
        public List<SharedVariable> SharedVariables { get; } = new();
        private readonly IVariableSource source;
        private readonly Object dirtyObject;
        public VariableSourceProxy(IVariableSource source, Object dirtyObject)
        {
            this.source = source;
            this.dirtyObject = dirtyObject;
        }
        public void Update()
        {
            source.SharedVariables.Clear();
            source.SharedVariables.AddRange(SharedVariables);
            EditorUtility.SetDirty(dirtyObject);
            AssetDatabase.SaveAssets();
        }
    }
    [CustomEditor(typeof(GameVariableScope))]
    public class GameVariableScopeEditor : UnityEditor.Editor
    {
        private const string ButtonText = "Save Change";
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as GameVariableScope;
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            var proxy = new VariableSourceProxy(source, source);
            //Need attached to a virtual graphView to send event
            //It's an interesting hack so that you can use blackBoard outside of graphView
            var blackBoard = new AdvancedBlackBoard(proxy, new VirtualGraphView());
            foreach (var variable in source.SharedVariables)
            {
                //In play mode, use original variable to observe value change
                if (Application.isPlaying)
                {
                    blackBoard.AddSharedVariable(variable);
                }
                else
                {
                    blackBoard.AddSharedVariable(variable.Clone() as SharedVariable);
                }
            }
            blackBoard.style.position = Position.Relative;
            blackBoard.style.width = Length.Percent(100f);
            myInspector.Add(blackBoard);

            if (Application.isPlaying) return myInspector;

            var button = BehaviorTreeEditorUtility.GetButton(proxy.Update);
            button.clicked += () => { button.SetEnabled(false); };
            button.style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
            button.text = ButtonText;
            button.SetEnabled(false);
            blackBoard.RegisterCallback<VariableChangeEvent>(_ => button.SetEnabled(true));
            myInspector.Add(button);
            myInspector.Add(new PropertyField(serializedObject.FindProperty("parentScope"), "Parent Scope"));
            return myInspector;
        }
    }
    [CustomEditor(typeof(SceneVariableScope))]
    public class SceneVariableScopeEditor : UnityEditor.Editor
    {
        private const string ButtonText = "Save Change";
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as SceneVariableScope;
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            var proxy = new VariableSourceProxy(source, source);
            //Need attached to a virtual graphView to send event
            //It's an interesting hack so that you can use blackBoard outside of graphView
            var blackBoard = new AdvancedBlackBoard(proxy, new VirtualGraphView());
            foreach (var variable in source.SharedVariables)
            {
                //In play mode, use original variable to observe value change
                if (Application.isPlaying)
                {
                    blackBoard.AddSharedVariable(variable);
                }
                else
                {
                    blackBoard.AddSharedVariable(variable.Clone() as SharedVariable);
                }
            }
            blackBoard.style.position = Position.Relative;
            blackBoard.style.width = Length.Percent(100f);
            myInspector.Add(blackBoard);

            if (Application.isPlaying) return myInspector;

            var button = BehaviorTreeEditorUtility.GetButton(proxy.Update);
            button.clicked += () => { button.SetEnabled(false); };
            button.style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
            button.text = ButtonText;
            button.SetEnabled(false);
            blackBoard.RegisterCallback<VariableChangeEvent>(_ => button.SetEnabled(true));
            myInspector.Add(button);
            myInspector.Add(new PropertyField(serializedObject.FindProperty("parentScope"), "Parent Scope"));
            return myInspector;
        }
    }
}