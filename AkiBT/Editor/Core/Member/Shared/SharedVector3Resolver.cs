using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{

    public class SharedVector3Resovler :FieldResolver<SharedVector3Field,SharedVector3>
    {
        public SharedVector3Resovler(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedVector3Field CreateEditorField(FieldInfo fieldInfo)
        {
            var editorField = new SharedVector3Field(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(FieldInfo info) =>info.FieldType==typeof(SharedVector3) ;
         
    }
     public class SharedVector3Field : SharedVariableField<SharedVector3>
    {
         Vector3Field valueField;
        public SharedVector3Field(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
           
            valueField=new Vector3Field("Value");
            valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
            this.dropdownField.Add(valueField);

        }
        public override SharedVector3 value { get => base.value; set {base.value = value;UpdateValue();} }
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            textField.value=value.Name;
            valueField.value=value.Value;
        }
    }
}
