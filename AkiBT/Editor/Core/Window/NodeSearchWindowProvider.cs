using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private BehaviorTreeView graphView;
        private EditorWindow graphEditor;
        private readonly NodeResolver nodeResolver = new NodeResolver();
        private Texture2D _indentationIcon;
        private string[] showGroupNames;
        public void Initialize(BehaviorTreeView graphView, EditorWindow graphEditor,string[] showGroupNames)
        {
            this.graphView = graphView;
            this.graphEditor = graphEditor;
            this.showGroupNames=showGroupNames;
            _indentationIcon=new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }
        static readonly Type[] _Types={typeof(Action),typeof(Conditional),typeof(Composite),typeof(Decorator)};
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"),0));
            entries.Add(new SearchTreeEntry(new GUIContent("Create Group Block",_indentationIcon)) { level = 1, userData = typeof(GroupBlock) });
            List<Type> nodeTypes =SearchUtility.FindSubClassTypes(_Types);
            var groups=nodeTypes.GroupsByAkiGroup();;
            nodeTypes=nodeTypes.Except(groups.SelectMany(x=>x)).ToList();
            groups=groups.SelectString(showGroupNames);
            foreach(var _type in _Types)  
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {_type.Name}"),1));
                var group=groups.SelectFather(_type);
                foreach(var subGroup in group)
                {
                    entries.AddAllEntries(subGroup,_indentationIcon,2);
                }
                var left=nodeTypes.Where(x=>x.IsSubclassOf(_type));
                foreach(Type type in left)
                {
                    entries.AddEntry(type,2,_indentationIcon);
                }
            }
            return entries;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = this.graphEditor.rootVisualElement.ChangeCoordinatesTo(this.graphEditor.rootVisualElement.parent, context.screenMousePosition - this.graphEditor.position.position);
            var localMousePosition = this.graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            Rect newRect=new Rect(localMousePosition, new Vector2(100, 100));
            var type = searchTreeEntry.userData as Type;
            if(type==typeof(GroupBlock))
            {
                graphView.CreateCommentBlock(newRect);
                return true;
            }
            var node = this.nodeResolver.CreateNodeInstance(type,graphView);
            node.SetPosition(newRect);
            this.graphView.AddElement(node);
            node.onSelectAction=graphView.onSelectAction;
            return true;
        }
    }
    public class CertainNodeSearchWindowProvider<T> : ScriptableObject, ISearchWindowProvider where T:NodeBehavior
    {
        private BehaviorTreeNode node;
        private Texture2D _indentationIcon;
        private string[] showGroupNames;
        public void Init(BehaviorTreeNode node,string[] showGroupNames)
        {
            this.node = node;
            this.showGroupNames=showGroupNames;
            _indentationIcon=new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            Dictionary<string,List<Type>> attributeDict=new Dictionary<string, List<Type>>();

            entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {typeof(T).Name}"),0));
            List<Type> nodeTypes =SearchUtility.FindSubClassTypes(typeof(T));
            var groups=nodeTypes.GroupsByAkiGroup();//按AkiGroup进行分类
            nodeTypes=nodeTypes.Except(groups.SelectMany(x=>x)).ToList();//去除被分类的部分
            groups=groups.SelectString(showGroupNames);
            foreach(var group in groups)
            {
                entries.AddAllEntries(group,_indentationIcon,1);
            }
            foreach(Type type in nodeTypes)
            {
                entries.AddEntry(type,1,_indentationIcon);
            }
            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as System.Type;
            this.node.SetBehavior(type);
            return true;
        }
        
            
        }
    
    public class SearchUtility
    {
        public static List<Type> FindSubClassTypes(Type father)
        {
           return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(father) && !t.IsAbstract).ToList();
        }
       public static List<Type> FindSubClassTypes(Type[] fathers)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .Where(t => fathers.Any(f => t.IsSubclassOf(f)))
                            .Where(t => !t.IsAbstract)
                            .ToList();
        }
        const char Span='/';
         public static string[] GetSplittedGroupName (string group)
         {
            var array=group.Split(Span,StringSplitOptions.RemoveEmptyEntries) ;
            return array.Length>0?array: new string[1]{group};
         }
        
        
    
    }
    public static class SearchExtension
    {
        public static IEnumerable<IGrouping<string, Type>> GroupsByAkiGroup(this IEnumerable<Type> types)
        {
            return types.GroupBy(t=>
            {
                var array=t.GetCustomAttributes(typeof(AkiGroup), false) as AkiGroup[];
                return array.Length>0?SearchUtility.GetSplittedGroupName(array[0].Group)[0]:null;
            }).Where(x=>!string.IsNullOrEmpty(x.Key));
        }
        public static IEnumerable<IGrouping<string, Type>> SubGroups(this IGrouping<string, Type>group,int level)
        {
            return group.GroupBy(t=>
            {
                var array=t.GetCustomAttributes(typeof(AkiGroup), false) as AkiGroup[];
                var subcategory=SearchUtility.GetSplittedGroupName(array[0].Group);
                return subcategory.Length>level?subcategory[level]:null;
            }).Where(x=>!string.IsNullOrEmpty(x.Key));
        }
        public static IEnumerable<IGrouping<string, Type>> SelectFather(this IEnumerable<IGrouping<string, Type>> groups,Type Father)
        {
            return groups.SelectMany(x=>x).Where(x => x.IsSubclassOf(Father)).GroupsByAkiGroup();
        }
        public static IEnumerable<IGrouping<string, Type>> SelectString(this IEnumerable<IGrouping<string, Type>> groups,string[] showGroupNames)
        {
            return showGroupNames!=null?groups.Where(x=>showGroupNames.Any(a=>a==x.Key)):groups;

        }
        public static void AddEntry(this List<SearchTreeEntry> entries,Type _type,int _level,Texture icon)
        {
            entries.Add(new SearchTreeEntry(new GUIContent(_type.Name,icon)) { level = _level, userData = _type });
        }
        public static void AddAllEntries(this List<SearchTreeEntry> entries, IGrouping<string, Type> group,Texture icon,int level,int subCount=1)
        {
                entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group.Key}"),level));
                var subGroups=group.SubGroups(subCount);
                var left=group.Except(subGroups.SelectMany(x=>x));
                foreach(var subGroup in subGroups)
                {
                    entries.AddAllEntries(subGroup,icon,level+1,subCount+1);
                }
                foreach(Type type in left)
                {
                    entries.AddEntry(type,level+1,icon);
                }
        }
    }
}
