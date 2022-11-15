using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per GameObject
        private static readonly Dictionary<GameObject,GraphEditorWindow> cache = new Dictionary<GameObject, GraphEditorWindow>();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView=>graphView;
        private GameObject key { get; set; }
        InfoView infoView;
        public static void Show(BehaviorTree bt)
        {
            var window = Create(bt);
            window.Show();
            window.Focus();
        }

        private static GraphEditorWindow Create(BehaviorTree bt)
        {
            var key = bt.gameObject;
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var window = CreateInstance<GraphEditorWindow>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"行为树结点编辑器({bt.gameObject.name})");
            window.key = key;
            cache[key] = window;
            return window;
        }
        /// <summary>
        /// 构造视图
        /// </summary>
        /// <param name="window"></param>
        /// <param name="behaviorTree"></param>
        private static void StructGraphView(GraphEditorWindow window, BehaviorTree behaviorTree)
        {
            window.rootVisualElement.Clear();
            window.graphView = new BehaviorTreeView(behaviorTree, window);
            window.infoView=new InfoView();
            window.infoView.styleSheets.Add(Resources.Load<StyleSheet>("Info"));
            window.graphView.Add( window.infoView);
            window.graphView.onSelectAction=window.OnNodeSelectionChange;//绑定委托
            window.graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(window.graphView));
            window.rootVisualElement.Add(window.graphView);   
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
            AssetDatabase.CreateAsset(treeSO,$"{graphView.SavePath}/{key.name}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>AkiBT</color>外部行为树保存成功,SO生成位置:{graphView.SavePath}/{key.name}.asset\n{System.DateTime.Now.ToString()}");
        }
        
        private void OnDestroy()
        {
            if (key != null && cache.ContainsKey(key))
            {
                if(cache[key].GraphView.AutoSave)
                    cache[key].GraphView.Save(true);
                cache.Remove(key);
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
                StructGraphView(this, key.GetComponent<BehaviorTree>());
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
                        if (GUILayout.Button("保存到SO", EditorStyles.toolbarButton))
                        {
                            SaveDataToSO();
                        }
                        
                        graphView.SavePath=GUILayout.TextField(graphView.SavePath,GUILayout.Width(200));
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