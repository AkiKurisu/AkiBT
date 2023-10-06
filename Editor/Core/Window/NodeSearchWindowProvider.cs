using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private ITreeView graphView;
        private readonly NodeResolverFactory nodeResolver = NodeResolverFactory.Instance;
        private Texture2D _indentationIcon;
        private string[] showGroups;
        private string[] notShowGroups;
        public void Initialize(ITreeView graphView, (string[], string[]) mask)
        {
            this.graphView = graphView;
            showGroups = mask.Item1;
            notShowGroups = mask.Item2;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        private static readonly Type[] _Types = { typeof(Action), typeof(Conditional), typeof(Composite), typeof(Decorator) };
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0)
            };
            List<Type> nodeTypes = SubclassSearchUtility.FindSubClassTypes(_Types);
            var groups = nodeTypes.GroupsByAkiGroup(); ;
            nodeTypes = nodeTypes.Except(groups.SelectMany(x => x)).ToList();
            groups = groups.SelectGroup(showGroups).ExceptGroup(notShowGroups);
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
            entries.Add(new SearchTreeEntry(new GUIContent("Create Group Block", _indentationIcon)) { level = 1, userData = typeof(GroupBlock) });
            return entries;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = graphView.EditorWindow.rootVisualElement.ChangeCoordinatesTo(graphView.EditorWindow.rootVisualElement.parent, context.screenMousePosition - graphView.EditorWindow.position.position);
            var localMousePosition = graphView.View.contentViewContainer.WorldToLocal(worldMousePosition);
            Rect newRect = new(localMousePosition, new Vector2(100, 100));
            var type = searchTreeEntry.userData as Type;
            if (type == typeof(GroupBlock))
            {
                graphView.GroupBlockController.CreateBlock(newRect);
                return true;
            }
            var node = nodeResolver.Create(type, graphView);
            node.View.SetPosition(newRect);
            graphView.View.AddElement(node.View);
            node.OnSelectAction = graphView.OnNodeSelect;
            return true;
        }
    }
    public class CertainNodeSearchWindowProvider<T> : ScriptableObject, ISearchWindowProvider where T : NodeBehavior
    {
        private IBehaviorTreeNode node;
        private Texture2D _indentationIcon;
        private string[] showGroups;
        private string[] notShowGroups;
        public void Init(IBehaviorTreeNode node, (string[], string[]) mask)
        {
            this.node = node;
            showGroups = mask.Item1;
            notShowGroups = mask.Item2;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            Dictionary<string, List<Type>> attributeDict = new();

            entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {typeof(T).Name}"), 0));
            List<Type> nodeTypes = SubclassSearchUtility.FindSubClassTypes(typeof(T));
            var groups = nodeTypes.GroupsByAkiGroup();//按AkiGroup进行分类
            nodeTypes = nodeTypes.Except(groups.SelectMany(x => x)).ToList();//去除被分类的部分
            groups = groups.SelectGroup(showGroups).ExceptGroup(notShowGroups);
            foreach (var group in groups)
            {
                entries.AddAllEntries(group, _indentationIcon, 1);
            }
            foreach (Type type in nodeTypes)
            {
                entries.AddEntry(type, 1, _indentationIcon);
            }
            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            node.SetBehavior(type);
            return true;
        }
    }
}
