using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ObjectTypeSearchWindow : ScriptableObject, ISearchWindowProvider
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
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Select Object Type"), 0),
                new(new GUIContent("<No Constraint>", _indentationIcon)) { level = 1, userData = null }
            };
            List<Type> nodeTypes = SubclassSearchUtility.FindSubClassTypes(typeof(UnityEngine.Object));
            var groups = nodeTypes.GroupBy(t => t.Assembly);
            foreach (var group in groups)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group.Key.GetName().Name}"), 1));
                var subGroups = group.GroupBy(x => x.Namespace);
                foreach (var subGroup in subGroups)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {subGroup.Key}"), 2));
                    foreach (var type in subGroup)
                    {
                        entries.AddEntry(type, 3, _indentationIcon);
                    }
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
