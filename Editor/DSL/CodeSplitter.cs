using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
namespace Kurisu.AkiBT.DSL.Editor
{
    /// <summary>
    /// Tool to split fields and function, header file can be provided for UGC side.
    /// User do not need to know implementation detail.
    /// </summary>
    public class CodeSplitter : EditorWindow
    {
        private class GeneratorSetting
        {
            public bool convertFieldsToPublic;
            public string scriptFolderPath = string.Empty;
        }
        private GeneratorSetting setting;
        private static string KeyName => Application.productName + "_AkiBT_DSL_SplitterSetting";
        [MenuItem("Tools/AkiBT/DSL/Code Splitter")]
        private static void GetWindow()
        {
            GetWindowWithRect<CodeSplitter>(new Rect(0, 0, 400, 200));
        }
        private void OnEnable()
        {
            var data = EditorPrefs.GetString(KeyName);
            setting = JsonConvert.DeserializeObject<GeneratorSetting>(data);
            setting ??= new GeneratorSetting();
        }
        private void OnDisable()
        {
            EditorPrefs.SetString(KeyName, JsonConvert.SerializeObject(setting));
        }
        private void OnGUI()
        {
            setting.convertFieldsToPublic = EditorGUILayout.Toggle("Convert Fields To Public", setting.convertFieldsToPublic);
            if (GUILayout.Button("Select Script Folder"))
            {
                setting.scriptFolderPath = EditorUtility.OpenFolderPanel("Select Script Folder", "", "");
            }
            GUILayout.Label($"Path: {setting.scriptFolderPath}", new GUIStyle(GUI.skin.label) { wordWrap = true });
            if (GUILayout.Button("Split Code"))
            {
                SplitCode();
            }
            if (GUILayout.Button("Restore Code"))
            {
                RestoreCode();
            }
        }
        private void SplitCode()
        {
            if (string.IsNullOrEmpty(setting.scriptFolderPath))
            {
                Debug.LogError("Script folder path is not selected!");
                return;
            }
            string[] scriptFiles = Directory.GetFiles(setting.scriptFolderPath, "*.cs", SearchOption.AllDirectories);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            foreach (string scriptFile in scriptFiles)
            {
                string scriptContent = File.ReadAllText(scriptFile).TrimEnd();
                //ScriptName should be class name
                string className = Path.GetFileNameWithoutExtension(scriptFile);
                string namespaceName = GetNamespace(scriptContent);
                //Find last field index
                Type scriptType = types.FirstOrDefault(x => x.Name == className && x.Namespace == namespaceName);
                if (scriptType == null)
                {
                    Debug.Log($"Can not find type from {namespaceName}.{className}");
                    continue;
                }
                FieldInfo[] fields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                //Suppose class is public
                int classIndex = scriptContent.IndexOf("public class ");
                if (classIndex == -1)
                {
                    Debug.Log($"{namespaceName}.{className} is not public, generation was skipped");
                    continue;
                }
                int namespaceEndIndex = scriptContent.IndexOf('{');
                int endIndex = GetLastFieldIndex(fields, scriptContent);
                if (endIndex == -1)
                {
                    //No fields contained => use class '{' index
                    endIndex = scriptContent.IndexOf('{', namespaceEndIndex + 1) + 1;
                }
                string fieldContent = scriptContent[..endIndex];
                if (setting.convertFieldsToPublic)
                {
                    fieldContent = fieldContent.Replace("private ", "public ").Replace("internal ", "public ").Replace("protected ", "public ");
                }
                string methodContent = scriptContent[(endIndex + 1)..];
                //Replace to partial class
                string headerScriptContent = fieldContent.Replace("public class", "public partial class") + "\n}\n}";
                int indent = scriptContent.LastIndexOf('}', scriptContent.Length - 2) - scriptContent.LastIndexOf('\n', scriptContent.Length - 3) - 1;
                string bodyScriptContent = scriptContent[..(namespaceEndIndex + 1)] + "\n" + new string(' ', indent) + "public partial class " + className + "\n" + new string(' ', indent) + "{\n" + methodContent;
                string headerScriptPath = Path.GetDirectoryName(scriptFile) + "/" + className + "_Header.cs";
                string bodyScriptPath = Path.GetDirectoryName(scriptFile) + "/" + className + ".cs";
                string backUpPath = Path.GetDirectoryName(scriptFile) + "/" + className + "_Backup.txt";
                //Add backup for original script
                File.WriteAllText(backUpPath, scriptContent);
                File.WriteAllText(headerScriptPath, headerScriptContent);
                File.WriteAllText(bodyScriptPath, bodyScriptContent);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Code generation complete!");
        }
        private void RestoreCode()
        {
            if (string.IsNullOrEmpty(setting.scriptFolderPath))
            {
                Debug.LogError("Script folder path is not selected!");
                return;
            }
            string[] headerFiles = Directory.GetFiles(setting.scriptFolderPath, "*_Header.cs", SearchOption.AllDirectories);
            foreach (var file in headerFiles) File.Delete(file);
            string[] scriptFiles = Directory.GetFiles(setting.scriptFolderPath, "*_Backup.txt", SearchOption.AllDirectories);
            foreach (var file in scriptFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file)[..^7];
                string filePath = Path.Combine(Path.GetDirectoryName(file), $"{fileName}.cs");
                var scriptContent = File.ReadAllText(file);
                File.WriteAllText(filePath, scriptContent);
                File.Delete(file);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Code restoration complete!");
        }
        private static string GetNamespace(string scriptContent)
        {
            int namespaceIndex = scriptContent.IndexOf("namespace ");
            int namespaceEndIndex = scriptContent.IndexOf('{');
            return scriptContent[(namespaceIndex + 10)..namespaceEndIndex].TrimEnd();
        }
        private static int GetLastFieldIndex(FieldInfo[] fieldInfos, string scriptContent)
        {
            int endIndex = -1;
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                int index = scriptContent.IndexOf(" " + fieldInfo.Name + ";");
                if (index != -1)
                {
                    index += fieldInfo.Name.Length + 2;
                    if (index > endIndex)
                    {
                        endIndex = index;
                    }
                }
            }
            return endIndex;
        }

    }
}