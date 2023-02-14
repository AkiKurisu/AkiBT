using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace Kurisu.AkiBT.Editor
{
    public class DragDropManipulator : PointerManipulator
    {
        Object droppedObject = null;
        public event System.Action<Object> OnDragOverEvent;
        public DragDropManipulator(GraphView root)
        {
            target = root;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Register a callback when the user presses the pointer down.
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            // Register callbacks for various stages in the drag process.
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // Unregister all callbacks that you registered in RegisterCallbacksOnTarget().
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        // This method runs when a user presses a pointer down on the drop area.
        void OnPointerDown(PointerDownEvent _)
        {
            // Only do something if the window currently has a reference to an asset object.
            if (droppedObject != null)
            {
                // Clear existing data in DragAndDrop class.
                DragAndDrop.PrepareStartDrag();

                // Store reference to object and path to object in DragAndDrop static fields.
                DragAndDrop.objectReferences = new[] { droppedObject };
                // Start a drag.
                DragAndDrop.StartDrag(string.Empty);
            }
        }

        

        // This method runs if a user makes the pointer leave the bounds of the target while a drag is in progress.
        void OnDragLeave(DragLeaveEvent _)
        {
            droppedObject = null;
        }

        // This method runs every frame while a drag is in progress.
        void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        // This method runs when a user drops a dragged object onto the target.
        void OnDragPerform(DragPerformEvent _)
        {
            // Set droppedObject and draggedName fields to refer to dragged object.
            droppedObject = DragAndDrop.objectReferences[0];
            OnDragOverEvent?.Invoke(droppedObject);
            droppedObject=null;
        }
    
    }
}
