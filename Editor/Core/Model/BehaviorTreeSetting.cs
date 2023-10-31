using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    [Serializable]
    internal class EditorSetting
    {
        [Tooltip("Editor implementation that this setting belongs to, according to field `TreeEditorName` in IBehaviorTreeView")]
        public string EditorName;
        [AkiGroupSelector, Tooltip("Display type, filter AkiGroup according to this list, nodes without category will always be displayed")]
        public string[] ShowGroups;
        [AkiGroupSelector, Tooltip("The type that is not displayed, filter the AkiGroup according to this list, and the nodes without categories will always be displayed")]
        public string[] NotShowGroups;
        [Tooltip("You can customize the style of the Graph view")]
        public StyleSheet graphStyleSheet;
        [Tooltip("You can customize the style of the Inspector inspector")]
        public StyleSheet inspectorStyleSheet;
        [Tooltip("You can customize the style of Node nodes")]
        public StyleSheet nodeStyleSheet;
    }
    public class BehaviorTreeSetting : ScriptableObject
    {
        private const string k_BehaviorTreeSettingsPath = "Assets/AkiBTSetting.asset";
        private const string k_UserServiceSettingPath = "Assets/AkiBTUserServiceData.asset";
        private const string GraphFallBackPath = "AkiBT/Graph";
        private const string InspectorFallBackPath = "AkiBT/Inspector";
        private const string NodeFallBackPath = "AkiBT/Node";

        [SerializeField, Tooltip("Editor configuration, you can use different styles based on the editor name and provide filtering solutions for node search")]
        private EditorSetting[] settings;
        [SerializeField]
        private float autoLayoutSiblingDistance = 50f;
        [SerializeField, HideInInspector]
        private bool autoSave;
        [SerializeField, HideInInspector]
        private string lastPath;
        [SerializeField, HideInInspector]
        private BehaviorTreeServiceData serviceData;
        public BehaviorTreeServiceData ServiceData
        {
            get
            {
                if (serviceData == null)
                {
                    var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeServiceData)}");
                    if (guids.Length == 0)
                    {
                        serviceData = CreateInstance<BehaviorTreeServiceData>();
                        Debug.Log($"AkiBT User Service Data saving path : {k_UserServiceSettingPath}");
                        AssetDatabase.CreateAsset(serviceData, k_UserServiceSettingPath);
                        AssetDatabase.SaveAssets();
                    }
                    else serviceData = AssetDatabase.LoadAssetAtPath<BehaviorTreeServiceData>(AssetDatabase.GUIDToAssetPath(guids[0]));
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
        /// <summary>
        /// Auto node layout sibling distance
        /// </summary> <summary>
        /// 
        /// </summary>
        public float AutoLayoutSiblingDistance => autoLayoutSiblingDistance;
        public static StyleSheet GetGraphStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(GraphFallBackPath);
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.graphStyleSheet ?? Resources.Load<StyleSheet>(GraphFallBackPath);
        }
        public static StyleSheet GetInspectorStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(InspectorFallBackPath);
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.inspectorStyleSheet ?? Resources.Load<StyleSheet>(InspectorFallBackPath);
        }
        public static StyleSheet GetNodeStyle(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings == null || setting.settings.Length == 0 || !setting.settings.Any(x => x.EditorName.Equals(editorName))) return Resources.Load<StyleSheet>(NodeFallBackPath);
            var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
            return editorSetting.nodeStyleSheet ?? Resources.Load<StyleSheet>(NodeFallBackPath);
        }
        public static (string[], string[]) GetMask(string editorName)
        {
            var setting = GetOrCreateSettings();
            if (setting.settings.Any(x => x.EditorName.Equals(editorName)))
            {
                var editorSetting = setting.settings.First(x => x.EditorName.Equals(editorName));
                return (editorSetting.ShowGroups, editorSetting.NotShowGroups);
            }
            return (null, null);
        }
        public static BehaviorTreeSetting GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeSetting)}");
            BehaviorTreeSetting setting = null;
            if (guids.Length == 0)
            {
                setting = CreateInstance<BehaviorTreeSetting>();
                Debug.Log($"AkiBT Setting saving path : {k_BehaviorTreeSettingsPath}");
                AssetDatabase.CreateAsset(setting, k_BehaviorTreeSettingsPath);
                AssetDatabase.SaveAssets();
            }
            else setting = AssetDatabase.LoadAssetAtPath<BehaviorTreeSetting>(AssetDatabase.GUIDToAssetPath(guids[0]));
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
        private bool useReflection;
        private class Styles
        {
            public static GUIContent GraphEditorSettingStyle = new("Graph Editor Setting");
            public static GUIContent LayoutDistanceStyle = new("Layout Distance", "Auto node layout sibling distance");
            public static GUIContent EnableReflectionStyle = new("Enable Runtime Reflection",
                     "Set this on to map shared variables on awake automatically." +
                     " However, reflection may decrease your loading speed" +
                     " since shared variables will be mapped when behavior tree is first loaded"
                     );
        }
        private const string ReflectionSymbol = "AKIBT_REFLECTION";
        public BehaviorTreeSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = BehaviorTreeSetting.GetSerializedSettings();
            useReflection = ScriptingSymbolHelper.ContainsScriptingSymbol(ReflectionSymbol);
        }
        public override void OnGUI(string searchContext)
        {
            GUILayout.BeginVertical("Editor Settings", GUI.skin.box);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("settings"), Styles.GraphEditorSettingStyle);
            EditorGUILayout.PropertyField(m_Settings.FindProperty("autoLayoutSiblingDistance"), Styles.LayoutDistanceStyle);
            m_Settings.ApplyModifiedPropertiesWithoutUndo();
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Runtime Settings", GUI.skin.box);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            var newValue = EditorGUILayout.ToggleLeft(Styles.EnableReflectionStyle, useReflection);
            if (newValue != useReflection)
            {
                useReflection = newValue;
                if (useReflection)
                {
                    ScriptingSymbolHelper.AddScriptingSymbol(ReflectionSymbol);
                }
                else
                {
                    ScriptingSymbolHelper.RemoveScriptingSymbol(ReflectionSymbol);
                }
            }
            GUILayout.EndVertical();
        }
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {

            var provider = new BehaviorTreeSettingsProvider("Project/AkiBT Settings", SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
            return provider;

        }
    }
}