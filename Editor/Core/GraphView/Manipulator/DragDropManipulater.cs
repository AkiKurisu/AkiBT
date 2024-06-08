using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public abstract class DragDropManipulator : PointerManipulator
    {
        protected BehaviorTreeView TreeView => target as BehaviorTreeView;
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        // This method runs every frame while a drag is in progress.
        private void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        // This method runs when a user drops a dragged object onto the target.
        private void OnDragPerform(DragPerformEvent _)
        {
            // Set droppedObject and draggedName fields to refer to dragged object.
            if (DragAndDrop.objectReferences.Length == 0) return;
            OnDragOver(DragAndDrop.objectReferences, Event.current.mousePosition);
        }
        protected abstract void OnDragOver(Object[] droppedObjects, Vector2 mousePosition);
    }
}
