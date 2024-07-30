using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEngine;
namespace Kurisu.AkiBT.Example.Editor
{
    [CustomEditor(typeof(BehaviorTreeVM))]
    public class BehaviorTreeVMEditor : UnityEditor.Editor
    {
        private Button createButton;
        private Button clearButton;
        private Button runButton;
        private Button stopButton;
        private BehaviorTreeVM vm;
        private ObjectField codeAssetField;
        const string LabelText = "AkiBTVM";
        public override VisualElement CreateInspectorGUI()
        {
            vm = target as BehaviorTreeVM;
            VisualElement inspectorRoot = new();
            inspectorRoot.Add(VMEditorUtility.GetTitleLabel(LabelText));

            //Toggle
            var toggle = new Toggle("Is Playing")
            {
                bindingPath = "isPlaying"
            };
            toggle.SetEnabled(false);
            inspectorRoot.Add(toggle);
            //Input
            inspectorRoot.Add(codeAssetField = new ObjectField("Input Code") { objectType = typeof(TextAsset) });
            inspectorRoot.Add(new ObjectField("Output BehaviorTreeSO") { bindingPath = "behaviorTreeSO", objectType = typeof(BehaviorTreeSO) });
            //group
            var group1 = VMEditorUtility.GetGroup();
            var group2 = VMEditorUtility.GetGroup();
            //Button
            createButton = VMEditorUtility.GetButton("Compile", callBack: () => EditorApplication.update += Compile, widthPercent: 100);
            inspectorRoot.Add(createButton);

            var saveButton = VMEditorUtility.GetButton("Save", callBack: Save);
            group1.Add(saveButton);

            clearButton = VMEditorUtility.GetButton("Clear", callBack: vm.Clear);
            group1.Add(clearButton);
            inspectorRoot.Add(group1);

            runButton = VMEditorUtility.GetButton("Run", new Color(140 / 255f, 160 / 255f, 250 / 255f), vm.Run);
            group2.Add(runButton);

            stopButton = VMEditorUtility.GetButton("Stop", new Color(253 / 255f, 163 / 255f, 255 / 255f), vm.Stop);
            group2.Add(stopButton);
            inspectorRoot.Add(group2);

            return inspectorRoot;
        }
        private void Save()
        {
            if (vm.BehaviorTreeSO == null)
            {
                Debug.Log("<color=#ff2f2f>AkiBTVM</color>:VMBehaviorTreeSO Hasn't Created Yet!");
                return;
            }
            string path = EditorUtility.OpenFolderPanel("Select saving path", Application.dataPath, "");
            if (string.IsNullOrEmpty(path)) return;
            var savePath = path.Replace(Application.dataPath, string.Empty);
            string outPutPath = $"Assets/{savePath}/{vm.gameObject.name}.asset";
            AssetDatabase.CreateAsset(vm.BehaviorTreeSO, outPutPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>AkiBTVM</color>:BehaviorTreeSO saved succeed! File Path:{outPutPath}");
        }
        private void Compile()
        {
            EditorApplication.update -= Compile;
            var vm = target as BehaviorTreeVM;
            if (codeAssetField.value != null)
            {
                vm.Compile((codeAssetField.value as TextAsset).text);
            }
        }
    }
}
internal class VMEditorUtility
{
    internal static Button GetButton(string text, Color? color = null, System.Action callBack = null, float widthPercent = 50)
    {
        var button = new Button();
        if (callBack != null) button.clicked += callBack;
        if (color.HasValue) button.style.backgroundColor = color.Value;
        button.style.width = Length.Percent(widthPercent);
        button.text = text;
        button.style.fontSize = 15;
        return button;
    }
    internal static VisualElement GetGroup()
    {
        var group = new VisualElement();
        group.style.flexDirection = FlexDirection.Row;
        return group;
    }
    internal static Label GetTitleLabel(string text, int frontSize = 20)
    {
        var label = new Label(text);
        label.style.fontSize = frontSize;
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        return label;
    }
}

