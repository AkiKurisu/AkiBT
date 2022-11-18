using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{

    public class SharedVariableField<T> : BaseField<T> where T:SharedVariable
    {
        protected Foldout dropdownField;
        protected Toggle toggle;
        protected TextField textField;
        public SharedVariableField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput)
        {
            this.label=label;
            dropdownField=new Foldout();
            this.contentContainer.Add(dropdownField);
            toggle=new Toggle("Is Shared:");
            toggle.RegisterValueChangedCallback(evt => value.IsShared = evt.newValue);
            this.dropdownField.Add(toggle);
            textField=new TextField("Variable Name:");
            textField.RegisterValueChangedCallback(evt => value.Name = evt.newValue);
            this.dropdownField.Add(textField);
            this.dropdownField.value=false;
            this.dropdownField.text=$"{objectType.Name}";
        }
    }
    
}

