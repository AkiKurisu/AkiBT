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
    public class SharedIntField : SharedVariableField<SharedInt>
    {
         
         IntegerField valueField;
        public SharedIntField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
            valueField=new IntegerField("Value:");
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
     public class SharedVector3Field : SharedVariableField<SharedVector3>
    {
         Vector3Field valueField;
        public SharedVector3Field(string label, VisualElement visualInput, Type objectType) : base(label, visualInput,objectType)
        {
           
            valueField=new Vector3Field("Value:");
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

