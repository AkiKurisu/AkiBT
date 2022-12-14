using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per GameObject
        private static readonly Dictionary<int,GraphEditorWindow> cache = new Dictionary<int, GraphEditorWindow>();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView=>graphView;
        private IBehaviorTree key { get; set; }
        InfoView infoView;
        public static void Show(IBehaviorTree bt)
        {
            var window = Create(bt);
            window.Show();
            window.Focus();
        }

        private static GraphEditorWindow Create(IBehaviorTree bt)
        {
           
            var key = bt.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }
            var window = CreateInstance<GraphEditorWindow>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"行为树结点编辑器({bt._Object.name})");
            window.key = bt;
            cache[key] = window;
            return window;
        }
        /// <summary>
        /// 构造视图
        /// </summary>
        /// <param name="window"></param>
        /// <param name="behaviorTree"></param>
        private static void StructGraphView(GraphEditorWindow window, IBehaviorTree behaviorTree)
        {
            window.rootVisualElement.Clear();
            window.graphView = new BehaviorTreeView(behaviorTree, window);
            window.infoView=new InfoView("欢迎使用AkiBT,一个超简单的行为树!");
            window.infoView.styleSheets.Add((StyleSheet)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/AkiBT/Info.uss", typeof(StyleSheet)));
            window.graphView.Add( window.infoView);
            window.graphView.onSelectAction=window.OnNodeSelectionChange;//绑定委托
            GenerateBlackBoard(window.graphView);
            window.graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(window.graphView));
            window.rootVisualElement.Add(window.graphView);   
        }

      

        private static void GenerateBlackBoard(BehaviorTreeView _graphView)
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Shared Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent($"Int"), false, () => _graphView.AddPropertyToBlackBoard(new SharedInt(), false));
                menu.AddItem(new GUIContent($"Float"), false, () => _graphView.AddPropertyToBlackBoard(new SharedFloat(), false));
                menu.AddItem(new GUIContent($"Bool"), false, () => _graphView.AddPropertyToBlackBoard(new SharedBool(), false));
                menu.AddItem(new GUIContent($"Vector3"), false, () => _graphView.AddPropertyToBlackBoard(new SharedVector3(), false));
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
        void SaveDataToSO()
        {
            var treeSO=ScriptableObject.CreateInstance<BehaviorTreeSO>();
            if (!graphView.Save())
            {
                Debug.LogWarning($"<color=#ff2f2f>AkiBT</color>保存失败,不会生成ScritableObject\n{System.DateTime.Now.ToString()}");
                return;
            }
            graphView.Commit(treeSO);
            AssetDatabase.CreateAsset(treeSO,$"{graphView.SavePath}/{key._Object.name}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>AkiBT</color>外部行为树保存成功,SO生成位置:{graphView.SavePath}/{key._Object.name}.asset\n{System.DateTime.Now.ToString()}");
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
        private VisualElement CreateToolBar(BehaviorTreeView graphView)
        {
            return new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button("保存行为树", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = "成功更新行为树!";
                                this.ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = "无效行为树,请检查结点设置是否存在错误!";
                                this.ShowNotification(guiContent);
                            }
                        }
                        graphView.AutoSave=GUILayout.Toggle(graphView.AutoSave,"自动保存");
                        if(graphView.IsTree)
                        {
                            if (GUILayout.Button("保存到SO", EditorStyles.toolbarButton))
                            {
                                SaveDataToSO();
                            }
                            graphView.SavePath=GUILayout.TextField(graphView.SavePath,GUILayout.Width(200));
                        }
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