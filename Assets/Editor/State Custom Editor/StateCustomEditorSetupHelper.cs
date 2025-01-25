using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal class StateCustomEditorSetupHelper
{
    public StateCustomEditor editor;
    public void SetupCustomEditor()
    {
        try
        {
            editor.serializedObject.Update();
            this.InitDataFromTargetObject();
            this.CacheIcons();
            this.SetupSerialisedProperties();
        }
        catch 
        {
            //supress error as it doesnt seem to break things
        }
    }

    private void InitDataFromTargetObject()
    {
        this.editor.state = (State)editor.target;
        this.editor.stateName = this.editor.serializedObject.FindProperty("name");
        this.editor.stateDescription = this.editor.serializedObject.FindProperty("description");
        this.editor.stateConnections = this.editor.state.GetStateConnections();
        this.editor.EnterStateEvent = this.editor.serializedObject.FindProperty("EnterStateEvent");
        this.editor.BehaveEvent = this.editor.serializedObject.FindProperty("BehaveEvent");
        this.editor.ExitStateEvent = this.editor.serializedObject.FindProperty("ExitStateEvent");
    }

    private void CacheIcons()
    {
        this.editor.plusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
        this.editor.minusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
    }
    public void SetupSerialisedProperties()
    {
        
        SerializedProperty connectionListWrapper = editor.serializedObject.FindProperty(nameof(editor.state.stateConnections));
        this.editor.connectionListArrayReference = connectionListWrapper.FindPropertyRelative(nameof(editor.state.stateConnections.stateConnections));
        editor.connectionList = new SerializedProperty[editor.connectionListArrayReference.arraySize];
        editor.conditionLists = new SerializedProperty[editor.connectionListArrayReference.arraySize];


        for (int i = 0; i < editor.connectionListArrayReference.arraySize; i++)
        {
            editor.connectionList[i] = editor.connectionListArrayReference.GetArrayElementAtIndex(i);
            editor.conditionLists[i] = editor.connectionList[i].FindPropertyRelative("transitionConditions");
        }
    }
}

