using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [System.Serializable]
    internal class EditorSetting
    {
        [Tooltip("编辑器名称,默认编辑器则填入AkiBT")]
        public string EditorName;
        [Tooltip("显示的类型,根据该列表对AkiGroup进行筛选,无类别的结点始终会被显示")]
        public string[] ShowGroups;
        [Tooltip("不显示的类型,根据该列表对AkiGroup进行筛选,无类别的结点始终会被显示")]
        public string[] NotShowGroups;
        [Tooltip("你可以自定义Graph视图的样式")]
        public StyleSheet graphStyleSheet;
        [Tooltip("你可以自定义Inspector检查器的样式")]
        public StyleSheet inspectorStyleSheet;
        [Tooltip("你可以自定义Node结点的样式")]
        public StyleSheet nodeStyleSheet;
    }
    public class BehaviorTreeSetting : ScriptableObject
    {
        private const string k_BehaviorTreeSettingsPath = "Assets/AkiBTSetting.asset";
        private const string k_UserServiceSettingPath = "Assets/AkiBTUserServiceData.asset";
        private const string GraphFallBackPath="AkiBT/Graph";
        private const string InspectorFallBackPath="AkiBT/Inspector";
        private const string NodeFallBackPath="AkiBT/Node";

        [SerializeField,Tooltip("编辑器配置,你可以根据编辑器名称使用不同的样式,并为结点搜索提供筛选方案")]
        private EditorSetting[] settings;
        [SerializeField,HideInInspector]
        private bool autoSave;
        [SerializeField,HideInInspector]
        private string lastPath;
        [SerializeField,HideInInspector]
        private BehaviorTreeUserServiceData serviceData;
        public BehaviorTreeUserServiceData ServiceData
        {
            get
            {
                if(serviceData==null)
                {
                    var guids=AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeUserServiceData)}");
                    if(guids.Length==0)
                    {
                        serviceData = ScriptableObject.CreateInstance<BehaviorTreeUserServiceData>();
                        Debug.Log($"AkiBT User Service Data saving path : {k_UserServiceSettingPath}");
                        AssetDatabase.CreateAsset(serviceData, k_UserServiceSettingPath);
                        AssetDatabase.SaveAssets();
                    }
                    else serviceData=AssetDatabase.LoadAssetAtPath<BehaviorTreeUserServiceData>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                return serviceData;
            }
        }
        /// <summary>
        /// Cache last open folder path in editor
        /// </summary>
        /// <value></value>
        public string LastPath
        {
            get => lastPath;
            set => lastPath = value;

        }
        /// <summary>
        /// Cache user auto save option value
        /// </summary>
        /// <value></value>
        public bool AutoSave
        {
            get => autoSave;
            set => autoSave = value;
        }
        public static StyleSheet GetGraphStyle(string editorName)
        {
            var setting=GetOrCreateSettings();
            if(setting.settings==null||setting.settings.Length==0||!setting.settings.Any(x=>x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(GraphFallBackPath);
            var editorSetting=setting.settings.First(x=>x.EditorName.Equals(editorName));
            return editorSetting.graphStyleSheet??Resources.Load<StyleSheet>(GraphFallBackPath);
        } 
        public static StyleSheet GetInspectorStyle(string editorName)
        {
            var setting=GetOrCreateSettings();
            if(setting.settings==null||setting.settings.Length==0||!setting.settings.Any(x=>x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(InspectorFallBackPath);
            var editorSetting=setting.settings.First(x=>x.EditorName.Equals(editorName));
            return editorSetting.inspectorStyleSheet??Resources.Load<StyleSheet>(InspectorFallBackPath);
        } 
        public static StyleSheet GetNodeStyle(string editorName)
        {
            var setting=GetOrCreateSettings();
            if(setting.settings==null||setting.settings.Length==0||!setting.settings.Any(x=>x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(NodeFallBackPath);
            var editorSetting=setting.settings.First(x=>x.EditorName.Equals(editorName));
            return editorSetting.nodeStyleSheet??Resources.Load<StyleSheet>(NodeFallBackPath);
        } 
        public static (string[],string[]) GetMask(string editorName)
        {
            var setting=GetOrCreateSettings();
            if(setting.settings.Any(x=>x.EditorName.Equals(editorName))) 
            {
                var editorSetting=setting.settings.First(x=>x.EditorName.Equals(editorName));
                return (editorSetting.ShowGroups,editorSetting.NotShowGroups);
            }
            return (null,null);
        }
        public static BehaviorTreeSetting GetOrCreateSettings()
        {
            var guids=AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeSetting)}");
            BehaviorTreeSetting setting=null;
            if(guids.Length==0)
            {
                setting = ScriptableObject.CreateInstance<BehaviorTreeSetting>();
                Debug.Log($"AkiBT Setting saving path : {k_BehaviorTreeSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_BehaviorTreeSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting=AssetDatabase.LoadAssetAtPath<BehaviorTreeSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return setting;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    internal class BehaviorTreeSettingsProvider : SettingsProvider
    {
        private SerializedObject m_Settings;

        class Styles
        {
            public static GUIContent mask = new GUIContent("Editor Setting");
        }
        public BehaviorTreeSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) {}
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = BehaviorTreeSetting.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(m_Settings.FindProperty("settings"), Styles.mask);
            m_Settings.ApplyModifiedPropertiesWithoutUndo();
        }
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            
            var provider = new BehaviorTreeSettingsProvider("Project/AkiBT Settings", SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
            
        }
    }
}