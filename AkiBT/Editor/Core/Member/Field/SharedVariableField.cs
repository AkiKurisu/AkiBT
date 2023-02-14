using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    internal interface IFoldout
    {
        public Foldout foldout{get;}
    }
    internal interface IInitField
    {
        public void InitField(ITreeView treeView);
    }
    public abstract class SharedVariableField<T,K> : BaseField<T>,IFoldout,IInitField where T:SharedVariable<K>,new()
    {
        private readonly bool forceShared;
        public Foldout foldout{get;private set;}
        private Toggle toggle;
        private ITreeView treeView;
        private DropdownField nameDropdown;
        private SharedVariable bindExposedProperty;
        public SharedVariableField(string label, VisualElement visualInput, Type objectType,FieldInfo fieldInfo) : base(label, visualInput)
        {
            forceShared=fieldInfo.GetCustomAttribute<ForceSharedAttribute>()!=null;
            AddToClassList("SharedVariableField");
            foldout=new Foldout();
            foldout.value=false;
            foldout.text=$"{objectType.Name}";
            contentContainer.Add(foldout);
            toggle=new Toggle("Is Shared");
            toggle.RegisterValueChangedCallback(evt =>{ value.IsShared = evt.newValue;OnToggle(evt.newValue);});            
            if(forceShared)
            {
                toggle.value=true;
                return;
            } 
            foldout.Add(toggle);
        }
        public void InitField(ITreeView treeView)
        {
            this.treeView=treeView;
            treeView.OnPropertyNameChangeEvent+=(variable)=>
            {
                if(variable!=bindExposedProperty)return;
                nameDropdown.value=variable.Name;
                value.Name=variable.Name;
            };
            OnToggle(toggle.value);
        } 
        private static List<string> GetList(ITreeView treeView)
        {
            return treeView.ExposedProperties
            .Where(x=>x.GetType()==typeof(T))
            .Select(v => v.Name)
            .ToList();
        }
        private void BindProperty()
        {
            if(treeView==null)return;
            bindExposedProperty=treeView.ExposedProperties.Where(x=>x.GetType()==typeof(T)&&x.Name.Equals(value.Name)).FirstOrDefault();
        }
        private void OnToggle(bool IsShared){
            if(IsShared)
            {      
                RemoveNameDropDown();
                if(nameDropdown==null&&value!=null&&treeView!=null)AddNameDropDown();
                RemoveValueField();
            }
            else
            {
                RemoveNameDropDown();
                RemoveValueField();
                if(valueField==null)AddValueField();
            }
        }
        private void AddNameDropDown()
        {
            nameDropdown=new DropdownField("Variable Name",GetList(treeView),value.Name??string.Empty);
            nameDropdown.RegisterCallback<MouseEnterEvent>((evt)=>{nameDropdown.choices=GetList(treeView);});
            nameDropdown.RegisterValueChangedCallback(evt => {value.Name = evt.newValue;BindProperty();});
            foldout.Add(nameDropdown);
        }
        private void RemoveNameDropDown()
        {
            if(nameDropdown!=null)foldout.Remove(nameDropdown);
            nameDropdown=null;
        }
        private void RemoveValueField()
        {
            if(valueField!=null)foldout.Remove(valueField);
            valueField=null;
        }
        private void AddValueField()
        {
            valueField=CreateValueField();
            valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
            if(value!=null)valueField.value=value.Value;
            this.foldout.Add(valueField);
        }
        protected abstract BaseField<K> CreateValueField();
        public sealed override T value {get=>base.value; set {
            if(value!=null)base.value=value.Clone() as T;
            else base.value=new T();
            if(forceShared)base.value.IsShared=true;
            UpdateValue();
        } }
        protected BaseField<K>valueField{get;set;}
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            if(valueField!=null)valueField.value=value.Value;
            BindProperty();
            OnToggle(value.IsShared);
        }
    }
    
}

;