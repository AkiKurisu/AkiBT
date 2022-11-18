using System.Reflection;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
public class SharedIntResovler :FieldResolver<SharedIntField,SharedInt>
    {
        public SharedIntResovler(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override SharedIntField CreateEditorField(FieldInfo fieldInfo)
        {
            var editorField = new SharedIntField(fieldInfo.Name,null,fieldInfo.FieldType);
            return editorField;
        }
        public static bool IsAcceptable(FieldInfo info) =>info.FieldType ==typeof(SharedInt) ;
         
    }
public class SharedIntField : SharedVariableField<SharedInt>
    {
         
         IntegerField valueField;
        public SharedIntField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
            valueField=new IntegerField("Value");
            valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
            this.dropdownField.Add(valueField);
        }
        public override SharedInt value { get => base.value; set {base.value = value;UpdateValue();} }
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            textField.value=value.Name;
            valueField.value=value.Value;
        }
    }
}