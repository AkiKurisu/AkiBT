using System;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class EnumListField<T> : ListField<T> where T : Enum
    {
        public EnumListField(string label, Func<VisualElement> elementCreator, Func<object> valueCreator) : base(label, elementCreator, valueCreator)
        {

        }
        protected override ListView CreateListView()
        {
            void bindItem(VisualElement e, int i)
            {
                (e as EnumField).value = value[i];
                (e as EnumField).RegisterValueChangedCallback((x) => value[i] = (T)x.newValue);
            }
            VisualElement makeItem()
            {
                var field = elementCreator.Invoke();
                (field as EnumField).label = string.Empty;
                return field;
            }
            const int itemHeight = 20;
            var view = new ListView(value, itemHeight, makeItem, bindItem);
            return view;
        }
    }
}