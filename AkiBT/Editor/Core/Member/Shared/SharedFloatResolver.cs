using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
public class SharedFloatResovler :FieldResolver<SharedFloatField,SharedFloat>
    {
        public SharedFloatResovler(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedFloatField CreateEditorField(FieldInfo fieldInfo)
        {
            var editorField = new SharedFloatField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(FieldInfo info) =>info.FieldType==typeof(SharedFloat) ;
         
    }
  public class SharedFloatField : SharedVariableField<SharedFloat>
    {
         FloatField valueField;
        public SharedFloatField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
           
            valueField=new FloatField("Value:");
            valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
            this.dropdownField.Add(valueField);

        }
        public override SharedFloat value { get => base.value;set {base.value = value;UpdateValue();} }
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            textField.value=value.Name;
            valueField.value=value.Value;
        }
    }
}