using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(EntryState))]
public class MyScriptableObjectEditor : Editor
{
    private EntryState entryState;
    private void OnEnable()
    {
        this.entryState = (EntryState)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        this.DrawStateConnectionObjectData();
        
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
    
    private void DrawStateConnectionObjectData()
        {
            object objectReferenceValue;
    
    
            objectReferenceValue = EditorGUILayout.ObjectField("State To", this.entryState.stateConnection.StateTo, typeof(State), true);
    
            if (objectReferenceValue is State stateTo)
                if (stateTo != null)
                    this.entryState.stateConnection.StateTo = stateTo;
        }
}
