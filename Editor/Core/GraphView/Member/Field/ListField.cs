using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
namespace Kurisu.AkiBT.Editor
{
    public class ListField<T> : BaseField<List<T>>
    {
        protected readonly ListView listView;
        protected readonly Func<VisualElement> elementCreator;
        protected readonly Func<object> valueCreator;
        public ListField(string label, VisualElement visualInput, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, visualInput)
        {
            value ??= new List<T>();
            this.elementCreator = elementCreator;
            this.valueCreator = valueCreator;
            listView = CreateListView();
            listView.selectionType = SelectionType.Multiple;
            listView.reorderable = true;
            listView.showAddRemoveFooter = true;
            listView.Q<Button>("unity-list-view__add-button").clickable = new Clickable(() =>
            {
                value.Add((T)valueCreator.Invoke());
                listView.RefreshItems();
            });
            contentContainer.Add(listView);
        }
        protected virtual ListView CreateListView()
        {
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                (e as BaseField<T>).value = value[i];
                (e as BaseField<T>).RegisterValueChangedCallback((x) => value[i] = x.newValue);

            };
            Func<VisualElement> makeItem = () =>
            {
                var field = elementCreator.Invoke();
                if (field is BaseField<T>) (field as BaseField<T>).label = string.Empty;
                return field;
            };
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }
        public sealed override List<T> value
        {
            get => base.value; set
            {
                if (value != null) base.value = new List<T>(value);
                else base.value = new List<T>();
                UpdateValue();
            }
        }
        private void UpdateValue()
        {
            if (listView != null) { listView.itemsSource = value; listView.RefreshItems(); }
        }

    }
}