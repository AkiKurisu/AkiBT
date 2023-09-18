using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.IO;
namespace Kurisu.AkiBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per GameObject
        private static readonly Dictionary<int, GraphEditorWindow> cache = new();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView => graphView;
        private UnityEngine.Object Key { get; set; }
        private InfoView infoView;
        protected virtual Type SOType => typeof(BehaviorTreeSO);
        protected virtual string TreeName => "Behavior Tree";
        protected virtual string InfoText => "Welcome to AkiBT, an ultra-simple behavior tree !";
        private static BehaviorTreeSetting setting;
        private static BehaviorTreeSetting Setting
        {
            get
            {
                if (setting == null) setting = BehaviorTreeSetting.GetOrCreateSettings();
                return setting;
            }
        }
        [MenuItem("Tools/AkiBT/AkiBT Editor")]
        private static void ShowEditorWindow()
        {
            string path = EditorUtility.SaveFilePanel("Select ScriptableObject save path", Application.dataPath, "BehaviorTreeSO", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = path.Replace(Application.dataPath, string.Empty);
            var treeSO = CreateInstance<BehaviorTreeSO>();
            AssetDatabase.CreateAsset(treeSO, $"Assets/{path}");
            AssetDatabase.SaveAssets();
            Show(treeSO);
        }
        public static void Show(IBehaviorTree bt)
        {
            var window = Create<GraphEditorWindow>(bt);
            window.Show();
            window.Focus();
        }
        /// <summary>
        /// 创建GraphView实例
        /// </summary>
        /// <param name="behaviorTree"></param>
        /// <returns></returns>
        protected virtual BehaviorTreeView CreateView(IBehaviorTree behaviorTree)
        {
            return new BehaviorTreeView(behaviorTree, this);
        }
        /// <summary>
        /// 创建EditorWindow实例
        /// </summary>
        /// <param name="bt"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static T Create<T>(IBehaviorTree bt) where T : GraphEditorWindow
        {

            var key = bt._Object.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return (T)cache[key];
            }
            var window = CreateInstance<T>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"{window.graphView.TreeEditorName} ({bt._Object.name})");
            window.Key = bt._Object;
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
            window.graphView = window.CreateView(behaviorTree);
            window.infoView = new InfoView(window.InfoText);
            window.infoView.styleSheets.Add(Resources.Load<StyleSheet>("AkiBT/Info"));
            window.graphView.Add(window.infoView);
            window.graphView.onSelectAction = window.OnNodeSelectionChange;//绑定委托
            GenerateBlackBoard(window.graphView);
            window.graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(window.graphView));
            window.rootVisualElement.Add(window.graphView);
        }



        private static void GenerateBlackBoard(BehaviorTreeView _graphView)
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection { title = "Shared Variables" });
            blackboard.addItemRequested = _blackboard =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Int"), false, () => _graphView.AddExposedProperty(new SharedInt()));
                menu.AddItem(new GUIContent("Float"), false, () => _graphView.AddExposedProperty(new SharedFloat()));
                menu.AddItem(new GUIContent("Bool"), false, () => _graphView.AddExposedProperty(new SharedBool()));
                menu.AddItem(new GUIContent("Vector3"), false, () => _graphView.AddExposedProperty(new SharedVector3()));
                menu.AddItem(new GUIContent("String"), false, () => _graphView.AddExposedProperty(new SharedString()));
                menu.AddItem(new GUIContent("Object"), false, () => _graphView.AddExposedProperty(new SharedObject()));
                menu.ShowAsContext();
            };

            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField)element).text;
                var index = _graphView.ExposedProperties.FindIndex(x => x.Name == oldPropertyName);
                if (string.IsNullOrEmpty(newValue))
                {
                    blackboard.contentContainer.RemoveAt(index + 1);
                    _graphView.ExposedProperties.RemoveAt(index);
                    return;
                }
                if (_graphView.ExposedProperties.Any(x => x.Name == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "A variable with the same name already exists !",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.Name == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].Name = newValue;
                _graphView.NotifyEditSharedVariable(_graphView.ExposedProperties[targetIndex]);
                ((BlackboardField)element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10, 100, 300, 400));
            _graphView.Add(blackboard);
            _graphView._blackboard = blackboard;
        }
        private void SaveDataToSO(string path)
        {
            var treeSO = CreateInstance(SOType);
            if (!graphView.Save())
            {
                Debug.LogWarning($"<color=#ff2f2f>{graphView.TreeEditorName}</color> : Save failed, ScriptableObject wasn't created !\n{System.DateTime.Now.ToString()}");
                return;
            }
            graphView.Commit((IBehaviorTree)treeSO);
            AssetDatabase.CreateAsset(treeSO, $"Assets/{path}/{Key.name}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>{graphView.TreeEditorName}</color> : Save succeed, ScriptableObject created path : {path}/{Key.name}.asset\n{System.DateTime.Now.ToString()}");
        }

        private void OnDestroy()
        {
            int code = Key.GetHashCode();
            if (Key != null && cache.ContainsKey(code))
            {
                if (Setting.AutoSave)
                    cache[code].graphView.Save(true);
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
            if (Key != null)
            {
                if (Key is GameObject) StructGraphView(this, (Key as GameObject).GetComponent<IBehaviorTree>());
                else StructGraphView(this, Key as IBehaviorTree);
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
                        if (GUILayout.Button($"Save {TreeName}", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = $"Update {TreeName} Succeed!";
                                ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = $"Invalid {TreeName}, please check the node connection for errors!";
                                ShowNotification(guiContent);
                            }
                        }
                        bool newValue = GUILayout.Toggle(Setting.AutoSave, "Auto Save", EditorStyles.toolbarButton);
                        if (newValue != Setting.AutoSave)
                        {
                            Setting.AutoSave = newValue;
                            EditorUtility.SetDirty(setting);
                            AssetDatabase.SaveAssets();
                        }
                        if (graphView.CanSaveToSO)
                        {
                            if (GUILayout.Button("Save To SO", EditorStyles.toolbarButton))
                            {
                                string path = EditorUtility.OpenFolderPanel("Select ScriptableObject save path", Setting.LastPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    Setting.LastPath = path;
                                    SaveDataToSO(path.Replace(Application.dataPath, string.Empty));
                                }

                            }
                        }
                        if (GUILayout.Button("Copy From SO", EditorStyles.toolbarButton))
                        {
                            string path = EditorUtility.OpenFilePanel("Select ScriptableObject to copy", Setting.LastPath, "asset");
                            var data = LoadDataFromFile(path.Replace(Application.dataPath, string.Empty));
                            if (data != null)
                            {
                                Setting.LastPath = path;
                                EditorUtility.SetDirty(setting);
                                AssetDatabase.SaveAssets();
                                ShowNotification(new GUIContent("Data Dropped Succeed!"));
                                graphView.CopyFromOtherTree(data, new Vector3(400, 300));
                            }
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Auto Layout", EditorStyles.toolbarButton))
                        {
                            NodeAutoLayouter.Layout(new BehaviorTreeLayoutConvertor().Init(graphView));
                        }
                        if (GUILayout.Button("Save To Json", EditorStyles.toolbarButton))
                        {
                            var serializedData = graphView.SerializeTreeToJson();
                            string path = EditorUtility.SaveFilePanel("Select json file save path", Setting.LastPath, graphView.BehaviorTree._Object.name, "json");
                            if (!string.IsNullOrEmpty(path))
                            {
                                FileInfo info = new FileInfo(path);
                                Setting.LastPath = info.Directory.FullName;
                                EditorUtility.SetDirty(setting);
                                File.WriteAllText(path, serializedData);
                                Debug.Log($"<color=#3aff48>{GraphView.TreeEditorName}</color>:Save json file succeed!");
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                        }
                        if (GUILayout.Button("Copy From Json", EditorStyles.toolbarButton))
                        {
                            string path = EditorUtility.OpenFilePanel("Select json file to copy", Setting.LastPath, "json");
                            if (!string.IsNullOrEmpty(path))
                            {
                                FileInfo info = new FileInfo(path);
                                Setting.LastPath = info.Directory.FullName;
                                EditorUtility.SetDirty(setting);
                                AssetDatabase.SaveAssets();
                                var data = File.ReadAllText(path);
                                if (graphView.CopyFromJsonFile(data, new Vector3(400, 300)))
                                    ShowNotification(new GUIContent("Json file Read Succeed!"));
                                else
                                    ShowNotification(new GUIContent("Json file is in wrong format!"));
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            );
        }
        private IBehaviorTree LoadDataFromFile(string path)
        {
            try
            {
                return AssetDatabase.LoadAssetAtPath<BehaviorTreeSO>($"Assets/{path}");

            }
            catch
            {
                this.ShowNotification(new GUIContent($"Invalid Path:Assets/{path}, please pick ScriptableObject inherited from BehaviorTreeSO"));
                return null;
            }
        }
        private void OnNodeSelectionChange(IBehaviorTreeNode node)
        {
            infoView.UpdateSelection(node);
        }
    }
}