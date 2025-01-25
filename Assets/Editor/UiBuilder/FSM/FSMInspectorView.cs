using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
public class FSMInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FSMInspectorView, FSMInspectorView.UxmlTraits>
    {
        
    }

    private Editor editor;

    public Editor Editor => this.editor;

    public void UpdateSelection(FSMStateView fsmStateView)
    {
        base.Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        this.editor = Editor.CreateEditor(fsmStateView.State);

        IMGUIContainer container = new IMGUIContainer(() =>
        {
            this.editor.OnInspectorGUI();
        });
        base.Add(container);
        
    }

    public void EdgeSelected(FSMConnectionView fsmConnectionView)
    {
        base.Clear();
        
        UnityEngine.Object.DestroyImmediate(editor);
        this.editor = Editor.CreateEditor(((FSMStateView)fsmConnectionView.output.node).State);

        IMGUIContainer container = new IMGUIContainer(() =>
        {
            this.editor.OnInspectorGUI();
        });
        base.Add(container);
    }

    public new void Clear()
    {
        base.Clear();
    }
}
