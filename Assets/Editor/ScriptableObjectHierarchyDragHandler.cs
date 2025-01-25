using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ScriptableObjectHierarchyDragHandler
{
    static ScriptableObjectHierarchyDragHandler()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        Event evt = Event.current;

        // Ensure this logic is only processed during a DragPerform event
        if (evt.type != EventType.DragPerform)
            return;

        // Ensure the mouse position is within the selectionRect of the current object
        if (!selectionRect.Contains(evt.mousePosition))
            return;

        // Get the dragged object as a FiniteStateMachine
        FiniteStateMachine fsm = DragAndDrop.objectReferences[0] as FiniteStateMachine;

        if (fsm == null)
            return;

        // Get the target GameObject under the mouse
        GameObject target = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (target == null)
            return;

        // Try to get the FSMMonoComponent or add it if it doesn't exist
        if (target.TryGetComponent<FSMMonoComponent>(out FSMMonoComponent fsmMono))
        {
            fsmMono.FiniteStateMachine = fsm;
        }
        else
        {
            fsmMono = target.AddComponent<FSMMonoComponent>();
            fsmMono.FiniteStateMachine = fsm;
        }

        // Mark the event as used to prevent other callbacks from processing it
        evt.Use();
    }
}