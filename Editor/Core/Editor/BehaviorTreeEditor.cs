using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private HashSet<ObserveProxyVariable> observeProxies;
        private const string LabelText = "AkiBT BehaviorTree <size=12>Version1.4.2</size>";
        private const string ButtonText = "Edit BehaviorTree";
        private const string DebugText = "Debug BehaviorTree";
        private IBehaviorTree Tree => target as IBehaviorTree;
        public override VisualElement CreateInspectorGUI()
        {
            var myInspector = new VisualElement();
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
            observeProxies = BehaviorTreeEditorUtility.DrawSharedVariables(myInspector, Tree, target, this);
            var button = BehaviorTreeEditorUtility.GetButton(() => { GraphEditorWindow.Show(Tree); });
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
        private void OnDisable()
        {
            if (observeProxies == null) return;
            foreach (var proxy in observeProxies)
            {
                proxy.Dispose();
            }
        }
    }
    [CustomEditor(typeof(BehaviorTreeSO))]
    public class BehaviorTreeSOEditor : UnityEditor.Editor
    {
        private HashSet<ObserveProxyVariable> observeProxies;
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
            observeProxies = BehaviorTreeEditorUtility.DrawSharedVariables(myInspector, bt, target, this);
            if (!Application.isPlaying)
            {
                var button = BehaviorTreeEditorUtility.GetButton(() => { GraphEditorWindow.Show(bt); });
                button.style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
                button.text = ButtonText;
                myInspector.Add(button);
            }
            return myInspector;
        }
        private void OnDisable()
        {
            if (observeProxies == null) return;
            foreach (var proxy in observeProxies)
            {
                proxy.Dispose();
            }
        }
    }
    public class BehaviorTreeEditorUtility
    {
        internal static Button GetButton(System.Action clickEvent)
        {
            var button = new Button(clickEvent);
            button.style.fontSize = 15;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.color = Color.white;
            return button;
        }
        public static HashSet<ObserveProxyVariable> DrawSharedVariables(VisualElement parent, IVariableSource source, UnityEngine.Object target, UnityEditor.Editor editor)
        {
            var observeProxies = new HashSet<ObserveProxyVariable>();
            var factory = FieldResolverFactory.Instance;
            int count = source.SharedVariables.Count;
            if (count == 0) return observeProxies;
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
                var fieldResolver = factory.Create(variable.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public));
                var valueField = fieldResolver.GetEditorField(null);
                fieldResolver.Restore(variable);
                fieldResolver.RegisterValueChangeCallback((obj) =>
                {
                    var index = source.SharedVariables.FindIndex(x => x.Name == variable.Name);
                    source.SharedVariables[index].SetValue(obj);
                });
                if (Application.isPlaying)
                {
                    var observeProxy = variable.Observe();
                    observeProxy.OnValueChange += (x) => fieldResolver.Value = x;
                    fieldResolver.Value = variable.GetValue();
                    //Disable since you should only edit global variable in source
                    if (variable.IsGlobal) valueField.SetEnabled(false);
                    valueField.tooltip = "Global variable can only edited in source at runtime";
                    observeProxies.Add(observeProxy);
                }
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
                if (!Application.isPlaying)
                {
                    globalToggle.clicked += () =>
                    {
                        variable.IsGlobal = !variable.IsGlobal;
                        SetToggleButtonColor(globalToggle, variable.IsGlobal);
                        NotifyEditor();
                    };
                }
                SetToggleButtonColor(globalToggle, variable.IsGlobal);
                globalToggle.style.width = Length.Percent(15);
                globalToggle.style.height = 25;
                content.Add(globalToggle);
                //Delate Variable
                if (!Application.isPlaying)
                {
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
                }
                //Append to row
                grid.Add(content);
                //Append to folder
                foldout.Add(grid);
            }
            parent.Add(foldout);
            return observeProxies;
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