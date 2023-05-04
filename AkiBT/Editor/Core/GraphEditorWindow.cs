using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
namespace Kurisu.AkiBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per GameObject
        private static readonly Dictionary<int,GraphEditorWindow> cache = new Dictionary<int, GraphEditorWindow>();
        private BehaviorTreeView graphView;
        public BehaviorTreeView GraphView=>graphView;
        private UnityEngine.Object key { get; set; }
        InfoView infoView;
        /// <summary>
        /// 用于判断SO类型
        /// </summary>
        /// <returns></returns>
        protected virtual Type SOType=>typeof(BehaviorTreeSO);
        protected virtual string TreeName=>"行为树";
        protected virtual string InfoText=>"欢迎使用AkiBT,一个超简单的行为树!";
        [MenuItem("Tools/AkiBT Editor")]
        private static void ShowEditorWindow()
        {
            string path=EditorUtility.OpenFolderPanel("选择保存路径",Application.dataPath,"");
            if(string.IsNullOrEmpty(path))return;
            path=path.Replace(Application.dataPath,string.Empty);
            var treeSO=ScriptableObject.CreateInstance<BehaviorTreeSO>();
            AssetDatabase.CreateAsset(treeSO,"Assets/"+path+"BehaviorTreeSO.asset");
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
        protected static T Create<T>(IBehaviorTree bt)where T:GraphEditorWindow
        {
           
            var key = bt._Object.GetHashCode();
            if (cache.ContainsKey(key))
            {
                return (T)cache[key];
            }
            var window = CreateInstance<T>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"{window.TreeName}结点编辑器({bt._Object.name})");
            window.key = bt._Object;
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
            window.graphView=window.CreateView(behaviorTree);
            window.infoView=new InfoView(window.InfoText);
            window.infoView.styleSheets.Add(Resources.Load<StyleSheet>("AkiBT/Info"));
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
                menu.AddItem(new GUIContent("Int"), false, () => _graphView.AddPropertyToBlackBoard(new SharedInt()));
                menu.AddItem(new GUIContent("Float"), false, () => _graphView.AddPropertyToBlackBoard(new SharedFloat()));
                menu.AddItem(new GUIContent("Bool"), false, () => _graphView.AddPropertyToBlackBoard(new SharedBool()));
                menu.AddItem(new GUIContent("Vector3"), false, () => _graphView.AddPropertyToBlackBoard(new SharedVector3()));
                menu.AddItem(new GUIContent("String"), false, () => _graphView.AddPropertyToBlackBoard(new SharedString()));
                menu.ShowAsContext();
            };

            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                var index= _graphView.ExposedProperties.FindIndex(x=>x.Name==oldPropertyName);
                if(string.IsNullOrEmpty(newValue))
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
                _graphView.NotifyEditSharedVariable(_graphView.ExposedProperties[targetIndex]);
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,100,300,400));
            _graphView.Add(blackboard);
            _graphView._blackboard = blackboard;
        }
        void SaveDataToSO()
        {
            var treeSO=ScriptableObject.CreateInstance(SOType);
            if (!graphView.Save())
            {
                Debug.LogWarning($"<color=#ff2f2f>{graphView.treeEditorName}</color>保存失败,不会生成ScriptableObject\n{System.DateTime.Now.ToString()}");
                return;
            }
            graphView.Commit((IBehaviorTree)treeSO);
            AssetDatabase.CreateAsset(treeSO,$"Assets/{BehaviorTreeView.SavePath}/{key.name}.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"<color=#3aff48>{graphView.treeEditorName}</color>外部{TreeName}保存成功,SO生成位置:{BehaviorTreeView.SavePath}/{key.name}.asset\n{System.DateTime.Now.ToString()}");
        }
        
        private void OnDestroy()
        {
            int code=key.GetHashCode();
            if (key != null && cache.ContainsKey(code))
            {
                if(BehaviorTreeView.AutoSave)
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
                if(key is GameObject)StructGraphView(this, (key as GameObject).GetComponent<IBehaviorTree>());
                else StructGraphView(this, (key as IBehaviorTree));
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
                        if (GUILayout.Button($"保存{TreeName}", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = $"成功更新{TreeName}!";
                                this.ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = $"无效{TreeName},请检查结点设置是否存在错误!";
                                this.ShowNotification(guiContent);
                            }
                        }
                        BehaviorTreeView.AutoSave=GUILayout.Toggle(BehaviorTreeView.AutoSave,"Auto Save");
                        if(graphView.CanSaveToSO)
                        {
                            if (GUILayout.Button("Save To SO", EditorStyles.toolbarButton))
                            {
                                string path=EditorUtility.OpenFolderPanel("选择保存路径",Application.dataPath,"");
                                if(!string.IsNullOrEmpty(path))
                                {
                                    BehaviorTreeView.SavePath=path.Replace(Application.dataPath,string.Empty);
                                    SaveDataToSO();
                                }
                                
                            }
                        }
                        if (GUILayout.Button("Copy From SO", EditorStyles.toolbarButton))
                        {
                            string path=EditorUtility.OpenFilePanel("选择复制文件",Application.dataPath,"asset");
                            var data=LoadDataFromFile(path.Replace(Application.dataPath,string.Empty));
                            if(data!=null)
                            {
                                ShowNotification(new GUIContent("Data Dropped Successfully!"));
                                graphView.CopyFromOtherTree(data,Vector2.zero);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
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
                this.ShowNotification(new GUIContent($"Invalid Path:Assets/{path},please pick BehaviorTreeSO"));
                return null;
            }
        }
        void OnNodeSelectionChange(BehaviorTreeNode node)
        {
            infoView.UpdateSelection(node);
        }


    }
}