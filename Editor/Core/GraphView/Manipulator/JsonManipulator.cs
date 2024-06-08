using System.IO;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class JsonManipulator : DragDropManipulator
    {
        protected override void OnDragOver(Object[] droppedObjects, Vector2 mousePosition)
        {
            foreach (var data in droppedObjects)
            {
                if (data is TextAsset textAsset && Path.GetFileName(AssetDatabase.GetAssetPath(textAsset)).EndsWith(".json"))
                {
                    if (TreeView.CopyFromJson(textAsset.text, mousePosition))
                    {
                        TreeView.EditorWindow.ShowNotification(new GUIContent("Json dropped succeed!"));
                    }
                    else
                    {
                        TreeView.EditorWindow.ShowNotification(new GUIContent("Invalid dragged json!"));
                        break;
                    }
                }
            }
        }
    }
}