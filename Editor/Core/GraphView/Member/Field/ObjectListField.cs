using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class ObjectListField<T> : ListField<T> where T : UnityEngine.Object
    {
        public ObjectListField(string label, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, elementCreator, valueCreator)
        {
        }
        protected override ListView CreateListView()
        {
            void bindItem(VisualElement e, int i)
            {
                (e as ObjectField).value = value[i];
                (e as ObjectField).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            }
            VisualElement makeItem()
            {
                var field = elementCreator.Invoke();
                (field as ObjectField).label = string.Empty;
                (field as ObjectField).objectType = typeof(T);
                return field;
            }
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }
    }
}