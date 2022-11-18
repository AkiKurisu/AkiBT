using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{

    public class SharedBoolResovler :FieldResolver<SharedBoolField,SharedBool>
    {
        public SharedBoolResovler(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedBoolField CreateEditorField(FieldInfo fieldInfo)
        {
            var editorField = new SharedBoolField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(FieldInfo info) =>info.FieldType==typeof(SharedBool) ;
         
    }
     public class SharedBoolField : SharedVariableField<SharedBool>
    {
         Toggle valueField;
        public SharedBoolField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
           
            valueField=new Toggle("Value");
            valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
            this.dropdownField.Add(valueField);

        }
        public override SharedBool value { get => base.value; set {base.value = value;UpdateValue();} }
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            textField.value=value.Name;
            valueField.value=value.Value;
        }
    }
}
