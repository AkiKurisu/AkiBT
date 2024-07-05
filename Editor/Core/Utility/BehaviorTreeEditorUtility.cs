using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
namespace Kurisu.AkiBT.Editor
{
    public class BehaviorTreeEditorUtility
    {
        public static void SetRoot(IBehaviorTreeContainer behaviorTreeContainer, Root root)
        {
            behaviorTreeContainer.GetType()
            .GetField("root", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(behaviorTreeContainer, root);
        }
        public static bool TryGetExternalTree(IBehaviorTreeContainer behaviorTreeContainer, out IBehaviorTreeContainer externalTreeContainer)
        {
            externalTreeContainer = behaviorTreeContainer.GetType()
            .GetField("externalBehaviorTree", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(behaviorTreeContainer) as IBehaviorTreeContainer;
            return externalTreeContainer != null;
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