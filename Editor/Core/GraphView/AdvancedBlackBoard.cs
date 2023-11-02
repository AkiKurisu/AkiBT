using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class AdvancedBlackBoard : Blackboard, IBlackBoard
    {
        public Blackboard View => this;
        private readonly FieldResolverFactory fieldResolverFactory = FieldResolverFactory.Instance;
        private readonly ScrollView scrollView;
        public VisualElement RawContainer => scrollView;
        private readonly List<SharedVariable> sharedVariables;
        private readonly HashSet<ObserveProxyVariable> observeProxies = new();
        public AdvancedBlackBoard(IVariableSource variableSource, GraphView graphView) : base(graphView)
        {
            var header = this.Q("header");
            header.style.height = new StyleLength(50);
            Add(scrollView = new());
            scrollView.Add(new BlackboardSection { title = "Shared Variables" });
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDispose);
            sharedVariables = variableSource.SharedVariables;
            if (!Application.isPlaying) InitRequestDelegate();
        }
        private void OnDispose(DetachFromPanelEvent _)
        {
            foreach (var proxy in observeProxies)
            {
                proxy.Dispose();
            }
        }
        private void InitRequestDelegate()
        {
            addItemRequested = _blackboard =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Int"), false, () => AddSharedVariableWithNotify(new SharedInt()));
                menu.AddItem(new GUIContent("Float"), false, () => AddSharedVariableWithNotify(new SharedFloat()));
                menu.AddItem(new GUIContent("Bool"), false, () => AddSharedVariableWithNotify(new SharedBool()));
                menu.AddItem(new GUIContent("Vector3"), false, () => AddSharedVariableWithNotify(new SharedVector3()));
                menu.AddItem(new GUIContent("String"), false, () => AddSharedVariableWithNotify(new SharedString()));
                menu.AddItem(new GUIContent("Object"), false, () => AddSharedVariableWithNotify(new SharedObject()));
                menu.ShowAsContext();
            };
            editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;
                var index = sharedVariables.FindIndex(x => x.Name == oldPropertyName);
                if (string.IsNullOrEmpty(newValue))
                {
                    RawContainer.RemoveAt(index + 1);
                    sharedVariables.RemoveAt(index);
                    return;
                }
                if (sharedVariables.Any(x => x.Name == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "A variable with the same name already exists !",
                        "OK");
                    return;
                }
                var targetIndex = sharedVariables.FindIndex(x => x.Name == oldPropertyName);
                sharedVariables[targetIndex].Name = newValue;
                NotifyVariableChanged(sharedVariables[targetIndex], VariableChangeType.NameChange);
                ((BlackboardField)element).text = newValue;
            };

        }
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            contentContainer.style.height = layout.height - 50;
        }
        private void AddSharedVariableWithNotify(SharedVariable variable)
        {
            AddSharedVariable(variable);
            NotifyVariableChanged(variable, VariableChangeType.Create);
        }
        public void AddSharedVariable(SharedVariable variable)
        {
            if (string.IsNullOrEmpty(variable.Name)) variable.Name = variable.GetType().Name;
            var localPropertyName = variable.Name;
            int index = 1;
            while (sharedVariables.Any(x => x.Name == localPropertyName))
            {
                localPropertyName = $"{variable.Name}{index}";
                index++;
            }
            variable.Name = localPropertyName;
            sharedVariables.Add(variable);
            var container = new VisualElement();
            var field = new BlackboardField { text = localPropertyName, typeText = variable.GetType().Name };
            field.capabilities &= ~Capabilities.Deletable;
            field.capabilities &= ~Capabilities.Movable;
            if (Application.isPlaying)
            {
                field.capabilities &= ~Capabilities.Renamable;
            }
            FieldInfo info = variable.GetType().GetField("value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var fieldResolver = fieldResolverFactory.Create(info);
            var valueField = fieldResolver.GetEditorField(null);
            fieldResolver.Restore(variable);
            fieldResolver.RegisterValueChangeCallback((obj) =>
            {
                var index = sharedVariables.FindIndex(x => x.Name == variable.Name);
                sharedVariables[index].SetValue(obj);
                NotifyVariableChanged(variable, VariableChangeType.ValueChange);
            });
            if (Application.isPlaying)
            {
                var observe = variable.Observe();
                observe.OnValueChange += (x) => fieldResolver.Value = x;
                observeProxies.Add(observe);
                fieldResolver.Value = variable.GetValue();
                //Disable since you should only edit global variable in source
                if (variable.IsGlobal)
                {
                    valueField.SetEnabled(false);
                    valueField.tooltip = "Global variable can only edited in source at runtime";
                }
            }
            var placeHolder = new VisualElement();
            placeHolder.Add(valueField);
            if (variable is SharedObject sharedObject)
            {
                placeHolder.Add(GetConstraintField(sharedObject, (ObjectField)valueField));
            }
            var sa = new BlackboardRow(field, placeHolder);
            sa.AddManipulator(new ContextualMenuManipulator((evt) => BuildBlackboardMenu(evt, sa)));
            RawContainer.Add(sa);
        }
        private void NotifyVariableChanged(SharedVariable sharedVariable, VariableChangeType changeType)
        {
            using VariableChangeEvent changeEvent = VariableChangeEvent.GetPooled(sharedVariable, changeType);
            changeEvent.target = this;
            SendEvent(changeEvent);
        }
        private void BuildBlackboardMenu(ContextualMenuPopulateEvent evt, VisualElement element)
        {
            evt.menu.MenuItems().Clear();
            if (Application.isPlaying) return;
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Delate Variable", (a) =>
            {
                int index = RawContainer.IndexOf(element);
                var variable = sharedVariables[index - 1];
                sharedVariables.RemoveAt(index - 1);
                RawContainer.Remove(element);
                NotifyVariableChanged(variable, VariableChangeType.Delate);
                return;
            }));
        }
        private VisualElement GetConstraintField(SharedObject sharedObject, ObjectField objectField)
        {
            const string NonConstraint = "No Constraint";
            var placeHolder = new VisualElement();
            string constraintTypeName;
            try
            {
                objectField.objectType = Type.GetType(sharedObject.ConstraintTypeAQM, true);
                constraintTypeName = "Constraint Type : " + objectField.objectType.Name;
            }
            catch
            {
                objectField.objectType = typeof(UnityEngine.Object);
                constraintTypeName = NonConstraint;
            }
            var typeField = new Label(constraintTypeName);
            placeHolder.Add(typeField);
            var button = new Button()
            {
                text = "Change Constraint Type"
            };
            button.clicked += () =>
             {
                 var provider = ScriptableObject.CreateInstance<ObjectTypeSearchWindow>();
                 provider.Initialize((type) =>
                 {
                     if (type == null)
                     {
                         typeField.text = sharedObject.ConstraintTypeAQM = NonConstraint;
                         objectField.objectType = typeof(UnityEngine.Object);
                     }
                     else
                     {
                         objectField.objectType = type;
                         sharedObject.ConstraintTypeAQM = type.AssemblyQualifiedName;
                         typeField.text = "Constraint Type : " + type.Name;
                     }
                     NotifyVariableChanged(sharedObject, VariableChangeType.ValueChange);
                 });
                 SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
             };

            placeHolder.Add(button);
            return placeHolder;
        }
    }
}