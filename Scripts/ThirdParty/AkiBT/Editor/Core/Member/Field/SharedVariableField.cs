using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public abstract class SharedVariableField<T,K> : BaseField<T> where T:SharedVariable<K>,new()
    {
        private Foldout foldout;
        private Toggle toggle;
        private BehaviorTreeView treeView;
        private DropdownField nameDropdown;
        private SharedVariable bindExposedProperty;
        public SharedVariableField(string label, VisualElement visualInput, Type objectType) : base(label, visualInput)
        {
            this.label=label;
            foldout=new Foldout();
            foldout.value=false;
            foldout.text=$"{objectType.Name}";
            contentContainer.Add(foldout);
            toggle=new Toggle("Is Shared");
            toggle.RegisterValueChangedCallback(evt =>{ value.IsShared = evt.newValue;OnToggle(evt.newValue);});
            foldout.Add(toggle);
        }
        public void InitField(BehaviorTreeView treeView)
        {
            this.treeView=treeView;
            treeView.OnExposedPropertyNameChangeEvent+=(variable)=>
            {
                if(variable!=bindExposedProperty)return;
                nameDropdown.value=variable.Name;
                value.Name=variable.Name;
            };
            OnToggle(toggle.value);
        } 
        static List<string> GetList(BehaviorTreeView treeView)
        {
            return treeView.ExposedProperties
            .Where(x=>x.GetType().IsSubclassOf(typeof(SharedVariable<K>)))
            .Select(v => v.Name)
            .ToList();
        }
        private void BindProperty()
        {
            if(treeView==null)return;
            bindExposedProperty=treeView.ExposedProperties.Where(x=>x.GetType().IsSubclassOf(typeof(SharedVariable<K>))&&x.Name.Equals(value.Name)).FirstOrDefault();
        }
        private void OnToggle(bool IsShared){
            if(IsShared)
            {      
                if(nameDropdown==null&&value!=null&&treeView!=null)
                {
                    nameDropdown=new DropdownField("Variable Name",GetList(treeView),value.Name!=null?value.Name:string.Empty);
                    nameDropdown.RegisterCallback<MouseEnterEvent>((evt)=>{nameDropdown.choices=GetList(treeView);});
                    nameDropdown.RegisterValueChangedCallback(evt => {value.Name = evt.newValue;BindProperty();});
                    foldout.Add(nameDropdown);
                }
                if(valueField!=null)foldout.Remove(valueField);
                valueField=null;
            }
            else
            {
                if(nameDropdown!=null)foldout.Remove(nameDropdown);
                nameDropdown=null;
                if(valueField==null)
                {
                    valueField=CreateValueField();
                    valueField.RegisterValueChangedCallback(evt => value.Value = evt.newValue);
                    if(value!=null)valueField.value=value.Value;
                    this.foldout.Add(valueField);
                }
            }
        }
        protected abstract BaseField<K> CreateValueField();
        public sealed override T value {get=>base.value; set {if(base.value==null)base.value=new T();UpdateValue(value);} }
        protected BaseField<K>valueField{get;set;}
        void UpdateValue(T newValue)
        {
            value.IsShared=newValue.IsShared;
            value.Value=newValue.Value;
            value.Name=newValue.Name;
            toggle.value=value.IsShared;
            if(valueField!=null)valueField.value=value.Value;
            BindProperty();
            OnToggle(value.IsShared);
        }
    }
    
}

