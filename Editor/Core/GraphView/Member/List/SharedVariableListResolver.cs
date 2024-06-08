using System.Reflection;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    [ResolveChild]
    public class SharedVariableListResolver<T> : ListResolver<T> where T : SharedVariable, new()
    {
        public SharedVariableListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo, resolver)
        {
        }
        protected override ListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            return new SharedVariableListField<T>(fieldInfo.Name, () => childResolver.CreateField(), () => new T());
        }
        public static bool IsAcceptable(Type infoType, FieldInfo _)
        {
            if (infoType.IsGenericType && infoType.GetGenericTypeDefinition() == typeof(List<>) && infoType.GenericTypeArguments[0].IsSubclassOf(typeof(SharedVariable))) return true;
            if (infoType.IsArray && infoType.GetElementType().IsSubclassOf(typeof(SharedVariable))) return true;
            return false;
        }
    }
    public class SharedVariableListField<T> : ListField<T>, IBindableField where T : SharedVariable
    {
        private ITreeView treeView;
        public event Action<ITreeView> OnTreeViewInitEvent;
        public SharedVariableListField(string label, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, elementCreator, valueCreator)
        {

        }
        public void BindTreeView(ITreeView treeView)
        {
            this.treeView = treeView;
            OnTreeViewInitEvent?.Invoke(treeView);
        }
        protected override ListView CreateListView()
        {
            void bindItem(VisualElement e, int i)
            {
                (e as BaseField<T>).value = value[i];
                (e as BaseField<T>).RegisterValueChangedCallback((x) => value[i] = x.newValue);
            }
            VisualElement makeItem()
            {
                var field = elementCreator.Invoke();
                (field as BaseField<T>).label = string.Empty;
                if (treeView != null) (field as IBindableField).BindTreeView(treeView);
                OnTreeViewInitEvent += (view) => { (field as IBindableField).BindTreeView(view); };
                return field;
            }
            var view = new ListView(value, 20, makeItem, bindItem);
            return view;
        }

    }
}