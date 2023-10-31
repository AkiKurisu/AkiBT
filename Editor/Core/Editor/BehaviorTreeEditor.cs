using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private const string LabelText = "AkiBT BehaviorTree <size=12>Version1.4.2</size>";
        private const string ButtonText = "Edit BehaviorTree";
        private const string DebugText = "Open BehaviorTree (Runtime)";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var bt = target as IBehaviorTree;
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            var toggle = new PropertyField(serializedObject.FindProperty("updateType"), "Update Type");
            myInspector.Add(toggle);
            var field = new PropertyField(serializedObject.FindProperty("externalBehaviorTree"), "External BehaviorTree");
            myInspector.Add(field);
            BehaviorTreeEditorUtility.DrawSharedVariables(myInspector, bt, target, this);
            var button = BehaviorTreeEditorUtility.GetButton(() => { GraphEditorWindow.Show(bt); });
            if (!Application.isPlaying)
            {
                button.style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
                button.text = ButtonText;
            }
            else
            {
                button.text = DebugText;
                button.style.backgroundColor = new StyleColor(new Color(253 / 255f, 163 / 255f, 255 / 255f));
            }
            myInspector.Add(button);
            return myInspector;
        }
    }
    [CustomEditor(typeof(BehaviorTreeSO))]
    public class BehaviorTreeSOEditor : UnityEditor.Editor
    {
        private const string LabelText = "AkiBT BehaviorTreeSO <size=12>Version1.4.2</size>";
        private const string ButtonText = "Edit BehaviorTreeSO";
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
            var bt = target as IBehaviorTree;
            var label = new Label(LabelText);
            label.style.fontSize = 20;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            myInspector.Add(label);
            myInspector.styleSheets.Add(BehaviorTreeSetting.GetInspectorStyle("AkiBT"));
            myInspector.Add(new Label("BehaviorTree Description"));
            var description = new PropertyField(serializedObject.FindProperty("Description"), string.Empty);
            myInspector.Add(description);
            BehaviorTreeEditorUtility.DrawSharedVariables(myInspector, bt, target, this);
            if (!Application.isPlaying)
            {
                var button = BehaviorTreeEditorUtility.GetButton(() => { GraphEditorWindow.Show(bt); });
                button.style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
                button.text = ButtonText;
                myInspector.Add(button);
            }
            return myInspector;
        }
    }
    internal class BehaviorTreeEditorUtility
    {
        internal static Button GetButton(System.Action clickEvent)
        {
            var button = new Button(clickEvent);
            button.style.fontSize = 15;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.color = Color.white;
            return button;
        }
        internal static void DrawSharedVariables(VisualElement parent, IVariableSource source, UnityEngine.Object target, UnityEditor.Editor editor)
        {
            var factory = FieldResolverFactory.Instance;
            int count = source.SharedVariables.Count;
            if (count == 0) return;
            var foldout = new Foldout
            {
                value = false,
                text = "SharedVariables"
            };
            foreach (var variable in source.SharedVariables)
            {
                var grid = new Foldout
                {
                    text = $"{variable.GetType().Name}  :  {variable.Name}",
                    value = false
                };
                var content = new VisualElement();
                content.style.flexDirection = FlexDirection.Row;
                content.style.justifyContent = Justify.SpaceBetween;
                var valueField = factory.Create(variable.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                                        .GetEditorField(source.SharedVariables, variable);
                if (valueField is TextField field)
                {
                    field.multiline = true;
                    field.style.maxWidth = 250;
                    field.style.whiteSpace = WhiteSpace.Normal;
                }
                valueField.style.width = Length.Percent(70f);
                content.Add(valueField);
                if (variable is SharedObject sharedObject)
                {
                    var objectField = valueField as ObjectField;
                    try
                    {
                        objectField.objectType = Type.GetType(sharedObject.ConstraintTypeAQM, true);
                        grid.text = $"{variable.GetType().Name} ({objectField.objectType.Name})  :  {variable.Name}";
                    }
                    catch
                    {
                        objectField.objectType = typeof(UnityEngine.Object);
                    }
                }
                valueField.RegisterCallback<InputEvent>((e) =>
                {
                    NotifyEditor();
                });
                //Is Global Field
                var globalToggle = new Button()
                {
                    text = "Is Global"
                };
                globalToggle.clicked += () =>
                {
                    variable.IsGlobal = !variable.IsGlobal;
                    SetToggleButtonColor(globalToggle, variable.IsGlobal);
                    NotifyEditor();
                };
                SetToggleButtonColor(globalToggle, variable.IsGlobal);
                globalToggle.style.width = Length.Percent(15);
                globalToggle.style.height = 25;
                content.Add(globalToggle);
                //Delate Variable
                var deleteButton = new Button(() =>
                {
                    source.SharedVariables.Remove(variable);
                    foldout.Remove(grid);
                    if (source.SharedVariables.Count == 0)
                    {
                        parent.Remove(foldout);
                    }
                    NotifyEditor();
                })
                {
                    text = "Delate"
                };
                deleteButton.style.width = Length.Percent(10f);
                deleteButton.style.height = 25;
                content.Add(deleteButton);
                //Append to row
                grid.Add(content);
                //Append to folder
                foldout.Add(grid);
            }
            parent.Add(foldout);
            void NotifyEditor()
            {
                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(editor);
                AssetDatabase.SaveAssets();
            }
            void SetToggleButtonColor(Button button, bool isOn)
            {
                button.style.color = isOn ? Color.green : Color.gray;
            }
        }
    }
}