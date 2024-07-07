using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BlackBoardComponent))]
    public class BlackBoardComponentEditor : UnityEditor.Editor
    {
        private AdvancedBlackBoardController controller;
        public override VisualElement CreateInspectorGUI()
        {
            var source = target as BlackBoardComponent;
            var bb = source.GetBlackBoard();
            var myInspector = new VisualElement();
            myInspector.style.flexDirection = FlexDirection.Column;
            controller = new AdvancedBlackBoardController(bb,
             new AdvancedBlackBoard.BlackBoardSettings()
             {
                 showIsExposed = true,
                 showIsGlobalToggle = true,
             },
             () =>
             {
                 // commit to component
                 source.SetBlackBoardVariables(controller.SharedVariables);
                 EditorUtility.SetDirty(source);
                 AssetDatabase.SaveAssets();
             });
            myInspector.Add(controller.GetBlackBoard());
            if (Application.isPlaying) return myInspector;
            myInspector.RegisterCallback<DetachFromPanelEvent>(_ => controller.UpdateIfDirty());
            return myInspector;
        }
    }
}
