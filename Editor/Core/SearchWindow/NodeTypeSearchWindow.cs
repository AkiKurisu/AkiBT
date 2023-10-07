using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class NodeTypeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private Texture2D _indentationIcon;
        private Action<Type> typeSelectCallBack;
        public void Initialize(Action<Type> typeSelectCallBack)
        {
            this.typeSelectCallBack = typeSelectCallBack;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        static readonly Type[] _Types = { typeof(Action), typeof(Conditional), typeof(Composite), typeof(Decorator) };
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Select Node Type"), 0)
            };
            List<Type> nodeTypes = SubclassSearchUtility.FindSubClassTypes(_Types);
            var groups = nodeTypes.GroupsByAkiGroup(); ;
            nodeTypes = nodeTypes.Except(groups.SelectMany(x => x)).ToList();
            foreach (var _type in _Types)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {_type.Name}"), 1));
                var group = groups.SelectFather(_type);
                foreach (var subGroup in group)
                {
                    entries.AddAllEntries(subGroup, _indentationIcon, 2);
                }
                var left = nodeTypes.Where(x => x.IsSubclassOf(_type));
                foreach (Type type in left)
                {
                    entries.AddEntry(type, 2, _indentationIcon);
                }
            }
            return entries;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            typeSelectCallBack?.Invoke(type);
            return true;
        }
    }
}
