using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Kurisu.AkiBT.Example.Editor
{
    [CustomEditor(typeof(BehaviorTreeVM))]
    public class BehaviorTreeVmEditor : UEditor
    {
        private Button _createButton;

        private Button _disposeButton;

        private Button _runButton;

        private Button _stopButton;

        private BehaviorTreeVM _vm;

        private ObjectField _codeAssetField;

        private const string LabelText = "BehaviorTree VM";

        public override VisualElement CreateInspectorGUI()
        {
            _vm = (BehaviorTreeVM)target;
            VisualElement inspectorRoot = new();
            inspectorRoot.Add(UIElementUtility.GetTitleLabel(LabelText));
            // Input
            inspectorRoot.Add(_codeAssetField = new ObjectField("Input Code") { objectType = typeof(TextAsset) });
            
            // Button groups
            var group = UIElementUtility.GetGroup();
            _runButton = UIElementUtility.GetButton("Compile and Run", new Color(140 / 255f, 160 / 255f, 250 / 255f),
                () =>
                {
                    EditorApplication.delayCall += CompileAndRun;
                });
            group.Add(_runButton);
            _stopButton = UIElementUtility.GetButton("Stop", new Color(253 / 255f, 163 / 255f, 255 / 255f), _vm.Stop);
            group.Add(_stopButton);
            inspectorRoot.Add(group);

            return inspectorRoot;
        }

        private void CompileAndRun()
        {
            var vm = (BehaviorTreeVM)target;
            if (_codeAssetField.value != null)
            {
                vm.Compile(((TextAsset)_codeAssetField.value).text);
            }
        }
    }

    internal class UIElementUtility
    {
        internal static Button GetButton(string text, Color? color = null, System.Action callBack = null,
            float widthPercent = 50)
        {
            var button = new Button();
            if (callBack != null) button.clicked += callBack;
            if (color.HasValue) button.style.backgroundColor = color.Value;
            button.style.width = Length.Percent(widthPercent);
            button.text = text;
            button.style.fontSize = 15;
            button.style.color = Color.white;
            return button;
        }

        internal static VisualElement GetGroup()
        {
            var group = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            return group;
        }

        internal static Label GetTitleLabel(string text, int frontSize = 20)
        {
            var label = new Label(text)
            {
                style =
                {
                    fontSize = frontSize,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };
            return label;
        }
    }
}