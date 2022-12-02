using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kurisu.AkiBT.Editor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private BehaviorTreeView graphView;
        private EditorWindow graphEditor;
        private readonly NodeResolver nodeResolver = new NodeResolver();
        private Texture2D _indentationIcon;
        public void Initialize(BehaviorTreeView graphView, EditorWindow graphEditor)
        {
            this.graphView = graphView;
            this.graphEditor = graphEditor;
            _indentationIcon=new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"),0));
            List<Type> nodeTypes=new List<Type>();
            Dictionary<string,List<Type>> attributeDict=new Dictionary<string, List<Type>>();
            //遍历全部把Action Conditional Composite加入列表
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if ((type.IsSubclassOf(typeof(Action))|type.IsSubclassOf(typeof(Conditional))|type.IsSubclassOf(typeof(Composite))|type.IsSubclassOf(typeof(Decorator)))&&
                    !type.IsAbstract)
                    {
                        nodeTypes.Add(type);
                        AkiGroup[] array;
                        if ((array = (type.GetCustomAttributes(typeof(AkiGroup), false) as AkiGroup[])).Length > 0)
                        {
                            if(attributeDict.ContainsKey(array[0].Group))
                            {
                                attributeDict[array[0].Group].Add(type);
                            }
                            else
                            {
                                attributeDict.Add(array[0].Group,new List<Type>(){type});
                            }
                            nodeTypes.Remove(type);
                        }
                    }
                }
            }
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select Action"),1));     
            foreach(string group in attributeDict.Keys)
            {
                bool needGroup=false;
                foreach(Type type in attributeDict[group])
                {
                    if(type.IsSubclassOf(typeof(Action)))
                    {
                        if(!needGroup)entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group}"),2));
                        needGroup=true;
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name,_indentationIcon)) { level = 3, userData = type });
                    }
                }
            }
            foreach(Type nodeType in nodeTypes)
            {
                if (nodeType.IsSubclassOf(typeof(Action)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name,_indentationIcon)) { level = 2, userData = nodeType });
                }
            }
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select Condition"),1));
            
            foreach(string group in attributeDict.Keys)
            {
                bool needGroup=false;
                foreach(Type type in attributeDict[group])
                {
                    if(type.IsSubclassOf(typeof(Conditional)))
                    {
                        if(!needGroup)entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group}"),2));
                        needGroup=true;
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name,_indentationIcon)) { level = 3, userData = type });
                    }
                }
            }
            foreach(Type nodeType in nodeTypes)
            {
                if (nodeType.IsSubclassOf(typeof(Conditional)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name,_indentationIcon)) { level = 2, userData = nodeType });
                }
            }
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select Composite"),1));
            
            foreach(string group in attributeDict.Keys)
            {
                bool needGroup=true;
                foreach(Type type in attributeDict[group])
                {
                    if(type.IsSubclassOf(typeof(Composite)))
                    {
                        if(!needGroup)entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group}"),2));
                        needGroup=true;
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name,_indentationIcon)) { level = 3, userData = type });
                    }
                }
            }
            foreach(Type nodeType in nodeTypes)
            {
                if (nodeType.IsSubclassOf(typeof(Composite)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name,_indentationIcon)) { level = 2, userData = nodeType });
                }
            }
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select Decorator"),1));
            
            foreach(string group in attributeDict.Keys)
            {
                bool needGroup=false;
                foreach(Type type in attributeDict[group])
                {
                    if(type.IsSubclassOf(typeof(Decorator)))
                    {
                        if(!needGroup)entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group}"),2));
                        needGroup=true;
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name,_indentationIcon)) { level = 3, userData = type });
                    }
                }
            }
            foreach(Type nodeType in nodeTypes)
            {
                if (nodeType.IsSubclassOf(typeof(Decorator)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name,_indentationIcon)) { level = 2, userData = nodeType });
                }
            }
            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            var node = this.nodeResolver.CreateNodeInstance(type);
            var worldMousePosition = this.graphEditor.rootVisualElement.ChangeCoordinatesTo(this.graphEditor.rootVisualElement.parent, context.screenMousePosition - this.graphEditor.position.position);
            var localMousePosition = this.graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            node.SetPosition(new Rect(localMousePosition, new Vector2(100, 100)));
            this.graphView.AddElement(node);
            //修改新的Node结点部分
            node.onSelectAction=graphView.onSelectAction;
            return true;
        }

    }
    public class CertainNodeSearchWindowProvider<T> : ScriptableObject, ISearchWindowProvider where T:NodeBehavior
    {
        private BehaviorTreeNode node;
        protected virtual string nodeName{get;}
        private Texture2D _indentationIcon;
        public void Init(BehaviorTreeNode node)
        {
            this.node = node;
            _indentationIcon=new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            Dictionary<string,List<Type>> attributeDict=new Dictionary<string, List<Type>>();
            List<Type> nodeTypes=new List<Type>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {nodeName}"),0));
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(T))&& !type.IsAbstract)
                    {
                        nodeTypes.Add(type);
                        AkiGroup[] array;
                        if ((array = (type.GetCustomAttributes(typeof(AkiGroup), false) as AkiGroup[])).Length > 0)
                        {
                            if(attributeDict.ContainsKey(array[0].Group))
                            {
                                attributeDict[array[0].Group].Add(type);
                            }
                            else
                            {
                                attributeDict.Add(array[0].Group,new List<Type>(){type});
                            }
                            nodeTypes.Remove(type);
                        }
                    }
                }
            }
            foreach(string group in attributeDict.Keys)
            {
                bool needGroup=false;
                foreach(Type type in attributeDict[group])
                {
                    
                    if(!needGroup)entries.Add(new SearchTreeGroupEntry(new GUIContent($"Select {group}"),1));
                    needGroup=true;
                    entries.Add(new SearchTreeEntry(new GUIContent(type.Name,_indentationIcon)) { level = 2, userData = type });
                }
            }
            foreach(Type nodeType in nodeTypes)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(nodeType.Name,_indentationIcon)) { level = 1, userData = nodeType });
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
}