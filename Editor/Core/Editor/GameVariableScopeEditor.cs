using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(GameVariableScope))]
    public class GameVariableScopeEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as GameVariableScope;
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            var controller = new AdvancedBlackBoardController(source,
            new AdvancedBlackBoard.BlackBoardSettings()
            {
                autoExposed = true
            },
             () =>
            {
                EditorUtility.SetDirty(source);
                AssetDatabase.SaveAssets();
            });
            myInspector.Add(controller.GetBlackBoard());
            if (Application.isPlaying) return myInspector;
            myInspector.RegisterCallback<DetachFromPanelEvent>(_ => controller.UpdateIfDirty());
            myInspector.Add(new PropertyField(serializedObject.FindProperty("parentScope"), "Parent Scope"));
            return myInspector;
        }
    }
    [CustomEditor(typeof(SceneVariableScope))]
    public class SceneVariableScopeEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as SceneVariableScope;
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            var controller = new AdvancedBlackBoardController(source,
            new AdvancedBlackBoard.BlackBoardSettings()
            {
                autoExposed = true
            },
            () =>
            {
                EditorUtility.SetDirty(source);
                AssetDatabase.SaveAssets();
            });
            myInspector.Add(controller.GetBlackBoard());
            if (Application.isPlaying) return myInspector;
            myInspector.RegisterCallback<DetachFromPanelEvent>(_ => controller.UpdateIfDirty());
            myInspector.Add(new PropertyField(serializedObject.FindProperty("parentScope"), "Parent Scope"));
            return myInspector;
        }
    }
}