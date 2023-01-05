using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class SharedVariableField<T,K> : BaseField<T> where T:SharedVariable<K>
    {
        private Foldout foldout;
        private Toggle toggle;
        private BehaviorTreeView treeView;
        private DropdownField nameDropdown;
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
            OnToggle(toggle.value);
        } 
        static List<string> GetList(BehaviorTreeView treeView)
        {
            return treeView.ExposedProperties
            .Where(x=>x.GetType().IsSubclassOf(typeof(SharedVariable<K>)))
            .Select(v => v.Name)
            .ToList();
        }
        private void OnToggle(bool IsShared){
            if(IsShared)
            {      
                if(nameDropdown==null&&value!=null&&treeView!=null)
                {
                    nameDropdown=new DropdownField("Variable Name",GetList(treeView),value.Name!=null?value.Name:string.Empty);
                    nameDropdown.RegisterCallback<MouseEnterEvent>((evt)=>{nameDropdown.choices=GetList(treeView);});
                    nameDropdown.RegisterValueChangedCallback(evt => value.Name = evt.newValue);
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
        protected virtual BaseField<K> CreateValueField(){return null;}
        public override T value { get => base.value; set {base.value = value;UpdateValue();} }
        protected BaseField<K>valueField{get;set;}
        void UpdateValue()
        {
            toggle.value=value.IsShared;
            if(valueField!=null)valueField.value=value.Value;
            OnToggle(value.IsShared);
        }
    }
    
}

