using UnityEditor;
using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI ();

            if (GUILayout.Button("打开行为树"))
            {
                var bt = target as BehaviorTree;
                GraphEditorWindow.Show(bt);
            }
        }
    }

}