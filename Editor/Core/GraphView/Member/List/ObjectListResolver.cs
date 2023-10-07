using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Kurisu.AkiBT.Editor
{
    public class ObjectListResolver<T> :FieldResolver<ObjectListField<T>,List<T>> where T:UnityEngine.Object
    {
        protected readonly IFieldResolver childResolver;
        public ObjectListResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
            childResolver=new ObjectResolver(fieldInfo);
        }
        protected override ObjectListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new ObjectListField<T>(fieldInfo.Name,null,()=>childResolver.CreateField(),()=>null);
        }
    }
    public class ObjectListField<T> : ListField<T> where T:UnityEngine.Object
    {
        public ObjectListField(string label, VisualElement visualInput,Func<VisualElement>elementCreator,Func<object>valueCreator): base(label, visualInput,elementCreator,valueCreator)
        {
        }
        protected override ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as ObjectField).value=value[i];
                (e as ObjectField).RegisterValueChangedCallback((x)=>value[i]= (T)x.newValue);
            };
            Func<VisualElement>makeItem=()=>
            {
                var field=elementCreator.Invoke();
                (field as ObjectField).label=string.Empty;
                (field as ObjectField).objectType=typeof(T);
                return field;
            };
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }
        
    }
}