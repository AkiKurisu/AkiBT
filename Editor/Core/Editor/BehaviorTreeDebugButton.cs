using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    internal class BehaviorTreeDebugButton : Button
    {
        private const string ButtonText = "Edit BehaviorTree";
        private const string DebugText = "Debug BehaviorTree";
        public BehaviorTreeDebugButton(IBehaviorTree tree) : base(() => GraphEditorWindow.Show(tree))
        {
            style.fontSize = 15;
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.color = Color.white;
            if (!Application.isPlaying)
            {
                style.backgroundColor = new StyleColor(new Color(140 / 255f, 160 / 255f, 250 / 255f));
                text = ButtonText;
            }
            else
            {
                text = DebugText;
                style.backgroundColor = new StyleColor(new Color(253 / 255f, 163 / 255f, 255 / 255f));
            }
        }
    }
}