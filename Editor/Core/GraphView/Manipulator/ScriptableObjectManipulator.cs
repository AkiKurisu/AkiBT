using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class ScriptableObjectManipulator : DragDropManipulator
    {
        protected override void OnDragOver(Object[] droppedObjects, Vector2 mousePosition)
        {
            foreach (var data in droppedObjects)
            {
                if (data is ScriptableObject)
                {
                    if (data is IBehaviorTree behaviorTree)
                    {
                        TreeView.EditorWindow.ShowNotification(new GUIContent("Asset dropped succeed!"));
                        TreeView.CopyFromTree(behaviorTree, mousePosition);
                    }
                    else
                    {
                        TreeView.EditorWindow.ShowNotification(new GUIContent("Invalid dragged asset!"));
                        break;
                    }
                }
            }
        }
    }
}