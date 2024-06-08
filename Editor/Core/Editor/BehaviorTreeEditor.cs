using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private static readonly string LabelText = $"AkiBT BehaviorTree <size=12>{BehaviorTreeSetting.Version}</size>";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var tree = target as IBehaviorTree;
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            var toggle = new PropertyField(serializedObject.FindProperty("updateType"), "Update Type");
            myInspector.Add(toggle);
            var field = new PropertyField(serializedObject.FindProperty("externalBehaviorTree"), "External BehaviorTree");
            field.SetEnabled(!Application.isPlaying);
            myInspector.Add(field);
            if (tree.SharedVariables.Count(x => x.IsExposed) != 0)
            {
                myInspector.Add(new SharedVariablesFoldout(tree, target, this));
            }
            myInspector.Add(new BehaviorTreeDebugButton(tree));
            return myInspector;
        }
    }
    [CustomEditor(typeof(BehaviorTreeSO))]
    public class BehaviorTreeSOEditor : UnityEditor.Editor
    {
        private static readonly string LabelText = $"AkiBT BehaviorTreeSO <size=12>{BehaviorTreeSetting.Version}</size>";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var tree = target as IBehaviorTree;
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            myInspector.Add(new Label("Editor Description"));
            var description = new TextField(string.Empty)
            {
                multiline = true
            };
            description.style.minHeight = 60;
            description.BindProperty(serializedObject.FindProperty("description"));
            myInspector.Add(description);
            if (tree.SharedVariables.Count(x => x.IsExposed) != 0)
            {
                myInspector.Add(new SharedVariablesFoldout(tree, target, this));
            }
            myInspector.Add(new BehaviorTreeDebugButton(tree));
            return myInspector;
        }
    }
}