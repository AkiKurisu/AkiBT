using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [ResolveChild]
    public class SharedVariableListResolver<T> : FieldResolver<SharedVariableListField<T>, List<T>> where T : SharedVariable, new()
    {
        private readonly IFieldResolver childResolver;
        public SharedVariableListResolver(FieldInfo fieldInfo, IFieldResolver resolver) : base(fieldInfo)
        {
            childResolver = resolver;
        }
        SharedVariableListField<T> editorField;
        protected override void SetTree(ITreeView ownerTreeView)
        {
            editorField.InitField(ownerTreeView);
        }
        protected override SharedVariableListField<T> CreateEditorField(FieldInfo fieldInfo)
        {
            editorField = new SharedVariableListField<T>(fieldInfo.Name, null, () => childResolver.CreateField(), () => new T());
            return editorField;
        }
        public static bool IsAcceptable(Type infoType, FieldInfo info) =>
        FieldResolverFactory.IsList(infoType) &&
        infoType.GenericTypeArguments.Length > 0 &&
        infoType.GenericTypeArguments[0].IsSubclassOf(typeof(SharedVariable));

    }
    public class SharedVariableListField<T> : ListField<T>, IInitField where T : SharedVariable
    {
        private ITreeView treeView;
        public event System.Action<ITreeView> OnTreeViewInitEvent;
        public SharedVariableListField(string label, VisualElement visualInput, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, visualInput, elementCreator, valueCreator)
        {

        }
        public void InitField(ITreeView treeView)
        {
            this.treeView = treeView;
            OnTreeViewInitEvent?.Invoke(treeView);
        }
        protected override ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as BaseField<T>).value = value[i];
                (e as BaseField<T>).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            };
            Func<VisualElement> makeItem = () =>
            {
                var field = elementCreator.Invoke();
                (field as BaseField<T>).label = string.Empty;
                if (treeView != null) (field as IInitField).InitField(treeView);
                OnTreeViewInitEvent += (view) => { (field as IInitField).InitField(view); };
                return field;
            };
            var view = new ListView(value, 60, makeItem, bindItem);
            return view;
        }

    }
}