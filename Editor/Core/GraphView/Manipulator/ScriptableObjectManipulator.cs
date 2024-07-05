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
                    if (data is IBehaviorTreeContainer container)
                    {
                        TreeView.EditorWindow.ShowNotification(new GUIContent("Asset dropped succeed!"));
                        TreeView.CopyFromTree(container.GetBehaviorTree(), mousePosition);
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