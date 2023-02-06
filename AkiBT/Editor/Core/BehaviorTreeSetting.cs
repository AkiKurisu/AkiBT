using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
[System.Serializable]
internal class BehaviorTreeNodeSearchMask
{
    [Tooltip("编辑器名称,如对默认编辑器添加遮罩则填入AkiBT")]
    public string EditorName="AkiBT";
    [Tooltip("显示的类型,根据该列表对AkiGroup进行筛选,无类别的结点始终会被显示")]
    public string[] ShowGroups;
}
public class BehaviorTreeSetting : ScriptableObject
{
    public const string k_BehaviorTreeSettingsPath = "Assets/AkiBTSetting.asset";

    [SerializeField,Tooltip("结点搜索遮罩,如果你有多个编辑器继承自AkiBT,可以在这里根据编辑器名称设置结点遮罩,这样在使用特定编辑器时可以隐藏不需要的结点")]
    private BehaviorTreeNodeSearchMask[] masks;
    public static string[] GetMask(string maskName)
    {
        var setting=GetOrCreateSettings();
        if(setting.masks.Any(x=>x.EditorName.Equals(maskName))) return setting.masks.First(x=>x.EditorName.Equals(maskName)).ShowGroups;
        return null;
    }
    internal static BehaviorTreeSetting GetOrCreateSettings()
    {
        var guids=AssetDatabase.FindAssets($"t:{nameof(BehaviorTreeSetting)}");
        BehaviorTreeSetting setting=null;
        if(guids.Length==0)
        {
            setting = ScriptableObject.CreateInstance<BehaviorTreeSetting>();
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

class BehaviorTreeSettingsProvider : SettingsProvider
{
    private SerializedObject m_Settings;

    class Styles
    {
        public static GUIContent mask = new GUIContent("Node Search Mask");
    }
    public BehaviorTreeSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) {}
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        m_Settings = BehaviorTreeSetting.GetSerializedSettings();
    }
    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.PropertyField(m_Settings.FindProperty("masks"), Styles.mask);
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