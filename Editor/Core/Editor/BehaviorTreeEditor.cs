using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTreeComponent))]
    public class BehaviorTreeComponentEditor : UnityEditor.Editor
    {
        private static readonly string LabelText = $"AkiBT BehaviorTree <size=12>{BehaviorTreeSetting.Version}</size>";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var tree = target as IBehaviorTreeContainer;
            // create instance for edit
            var instance = tree.GetBehaviorTree();
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetOrCreateSettings().GetInspectorStyle("AkiBT"));
            var toggle = new PropertyField(serializedObject.FindProperty("updateType"), "Update Type");
            myInspector.Add(toggle);
            var field = new PropertyField(serializedObject.FindProperty("externalBehaviorTree"), "External BehaviorTree");
            field.SetEnabled(!Application.isPlaying);
            myInspector.Add(field);
            if (instance.SharedVariables.Count(x => x.IsExposed) != 0)
            {
                myInspector.Add(new SharedVariablesFoldout(instance.BlackBoard, () =>
                {
                    // Not serialize data in playing mode
                    if (Application.isPlaying) return;
                    tree.SetBehaviorTreeData(instance.GetData());
                    EditorUtility.SetDirty(target);
                }));
            }
            myInspector.Add(new BehaviorTreeDebugButton(tree));
            return myInspector;
        }
    }
    [CustomEditor(typeof(BehaviorTreeAsset))]
    public class BehaviorTreeAssetEditor : UnityEditor.Editor
    {
        private static readonly string LabelText = $"AkiBT BehaviorTree <size=12>{BehaviorTreeSetting.Version}</size>";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var tree = target as IBehaviorTreeContainer;
            // create instance for edit
            var instance = tree.GetBehaviorTree();
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetOrCreateSettings().GetInspectorStyle("AkiBT"));
            myInspector.Add(new Label("Editor Description"));
            var description = new TextField(string.Empty)
            {
                multiline = true
            };
            description.style.minHeight = 60;
            description.BindProperty(serializedObject.FindProperty("description"));
            myInspector.Add(description);
            if (instance.SharedVariables.Count(x => x.IsExposed) != 0)
            {
                myInspector.Add(new SharedVariablesFoldout(instance.BlackBoard, () =>
                {
                    // Not serialize data in playing mode
                    if (Application.isPlaying) return;
                    tree.SetBehaviorTreeData(instance.GetData());
                    EditorUtility.SetDirty(target);
                }));
            }
            myInspector.Add(new BehaviorTreeDebugButton(tree));
            return myInspector;
        }
    }
}