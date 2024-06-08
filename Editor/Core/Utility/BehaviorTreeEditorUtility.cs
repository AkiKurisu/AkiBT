using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeEditorUtility
    {
        public static void SetRoot(IBehaviorTree behaviorTree, Root root)
        {
            behaviorTree.GetType()
            .GetField("root", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(behaviorTree, root);
        }
        public static bool TryGetExternalTree(IBehaviorTree behaviorTree, out IBehaviorTree externalTree)
        {
            externalTree = behaviorTree.GetType()
            .GetField("externalBehaviorTree", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(behaviorTree) as IBehaviorTree;
            return externalTree != null;
        }
        internal static Button GetButton(System.Action clickEvent)
        {
            var button = new Button(clickEvent);
            button.style.fontSize = 15;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.color = Color.white;
            return button;
        }
        public static string GetRelativePath(string path)
        {
            return path.Replace(Application.dataPath, "Assets/");
        }
    }
}