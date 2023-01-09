using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using Kurisu.AkiBT;
using Kurisu.AkiBT.Editor;
namespace Kurisu.AkiST.Editor
{
public class SkillEditorWindow : EditorWindow
{
        private static readonly Dictionary<int,SkillEditorWindow> cache = new Dictionary<int, SkillEditorWindow>();
        private SkillTreeView graphView;
        public SkillTreeView GraphView=>graphView;
        private IBehaviorTree key { get; set; }
        InfoView infoView;
        public static void Show(IBehaviorTree bt)
        {
            var window = Create(bt);
            window.Show();
            window.Focus();
        }

        private static SkillEditorWindow Create(IBehaviorTree bt)
        {
           
            var key = bt.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            var window = CreateInstance<SkillEditorWindow>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"技能树结点编辑器({bt._Object.name})");
            window.key = bt;
            cache[key] = window;
            return window;
        }
        /// <summary>
        /// 构造视图
        /// </summary>
        /// <param name="window"></param>
        /// <param name="behaviorTree"></param>
        private static void StructGraphView(SkillEditorWindow window, IBehaviorTree behaviorTree)
        {
            window.rootVisualElement.Clear();
            window.graphView = new SkillTreeView(behaviorTree, window);
            window.infoView=new InfoView("欢迎使用AkiST,一个针对技能系统优化的行为树版本!");
            window.infoView.styleSheets.Add((StyleSheet)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/AkiBT/Info.uss", typeof(StyleSheet)));
            window.graphView.Add( window.infoView);
            window.graphView.onSelectAction=window.OnNodeSelectionChange;//绑定委托
            GenerateBlackBoard(window.graphView);
            window.graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(window.graphView));
            window.rootVisualElement.Add(window.graphView);   
        }

      

        private static void GenerateBlackBoard(SkillTreeView _graphView)
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Shared Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent($"Int"), false, () => _graphView.AddPropertyToBlackBoard(new SharedInt()));
                menu.AddItem(new GUIContent($"Float"), false, () => _graphView.AddPropertyToBlackBoard(new SharedFloat()));
                menu.AddItem(new GUIContent($"Bool"), false, () => _graphView.AddPropertyToBlackBoard(new SharedBool()));
                menu.AddItem(new GUIContent($"Vector3"), false, () => _graphView.AddPropertyToBlackBoard(new SharedVector3()));
                menu.ShowAsContext();
            };
            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                var index= _graphView.ExposedProperties.FindIndex(x=>x.Name==oldPropertyName);
                if(newValue=="")
                {
                   blackboard.contentContainer.RemoveAt(index+1);
                   _graphView.ExposedProperties.RemoveAt(index);
                   return;
                }
                if (_graphView.ExposedProperties.Any(x => x.Name == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "同名变量已存在.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.Name == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].Name = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,100,300,400));
            _graphView.Add(blackboard);
            _graphView._blackboard = blackboard;
        }
        
        private void OnDestroy()
        {
            int code=key.GetHashCode();
            if (key != null && cache.ContainsKey(code))
            {
                if(cache[code].GraphView.AutoSave)
                    cache[code].GraphView.Save(true);
                cache.Remove(code);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void Reload()
        {
            if (key != null)
            {
                StructGraphView(this, key);
                Repaint();
            }
        }
        private VisualElement CreateToolBar(SkillTreeView graphView)
        {
            return new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button("保存技能树", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = "成功更新技能树!";
                                this.ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = "无效技能树,请检查结点设置是否存在错误!";
                                this.ShowNotification(guiContent);
                            }
                        }
                        graphView.AutoSave=GUILayout.Toggle(graphView.AutoSave,"自动保存");
                    }
                    
                   
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            );

        }
        void OnNodeSelectionChange(BehaviorTreeNode node)
        {
            infoView.UpdateSelection(node);
        }


}
}