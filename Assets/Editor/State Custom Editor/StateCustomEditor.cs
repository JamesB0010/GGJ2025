using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

//Custom editor for states
[CustomEditor(typeof(State))]
class StateCustomEditor : Editor
{
    //Attributes
    internal State state;

    //this is a reference to the array its self
    internal SerializedProperty connectionListArrayReference;

    //this is an array of serialised properties. its the same as the transition list but faster to access
    internal SerializedProperty[] connectionList;

    //this is a list of references to the condition list found on each state connection
    internal SerializedProperty[] conditionLists;

    //this is the connection list but its full of state connections rather than serializedproperties
    internal List<StateConnection> stateConnections = new List<StateConnection>();


    internal GUIContent plusIcon;
    internal GUIContent minusIcon;

    public SerializedProperty stateName;
    public SerializedProperty stateDescription;

    private StateCustomEditorSetupHelper setupHelper = new StateCustomEditorSetupHelper();
    private Vector2 scrollPosition = new Vector2();
    private List<I_TransitionEvaluator> evaluators = new List<I_TransitionEvaluator>();

    public SerializedProperty ExitStateEvent { get; set; }

    public SerializedProperty BehaveEvent { get; set; }

    public SerializedProperty EnterStateEvent { get; set; }
    //Methods

    //setup methods
    private void OnEnable()
    {
        this.setupHelper.editor = this;

        this.setupHelper.SetupCustomEditor();
        
        this.stateConnections.ForEach(connection =>
        {
            if (connection.transitionHandleMethod != TransitionHandleMethod.handledThroughInterfaceComponent)
                return;
            
            if(connection.AgentPrefab != null)
                this.GetEvaluatorsOnAgentPrefab(connection.AgentPrefab);
        });
    }


    //Drawing the Ui methods
    public override void OnInspectorGUI()
    {
        if (serializedObject == null)
            return;
        
        
        serializedObject.Update();

        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
        DrawStateNameAndDescription();

        this.DrawUnityEvents();

        DrawConnectionsListFoldout();

        bool connectionsFoldoutIsExpanded = this.connectionListArrayReference.isExpanded;
        if (connectionsFoldoutIsExpanded)
        {
            DrawStateConnectionsFoldoutContent();
        }
        
        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private void DrawUnityEvents()
    {
        EditorGUILayout.PropertyField(this.EnterStateEvent);

        EditorGUILayout.PropertyField(this.BehaveEvent);

        EditorGUILayout.PropertyField(this.ExitStateEvent);
    }

    private void DrawStateNameAndDescription()
    {
        DrawStateName();
        EditorGUILayout.LabelField("Description");
        this.state.Description = EditorGUILayout.TextArea(this.state.Description);

    }

    private void DrawStateName()
    {
        string nameFieldControlName = "nameField";
        GUI.SetNextControlName(nameFieldControlName);
        this.state.name = EditorGUILayout.TextField("name", this.state.name);

        if (Event.current.keyCode != KeyCode.Return)
            return;

        if (GUI.GetNameOfFocusedControl() != nameFieldControlName)
            return;
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawConnectionsListFoldout()
    {
        EditorGUILayout.BeginHorizontal();

        this.connectionListArrayReference.isExpanded =
            EditorGUILayout.Foldout(this.connectionListArrayReference.isExpanded, "State Connections");

        EditorGUILayout.IntField(this.connectionListArrayReference.arraySize, GUILayout.Width(47.5f));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawStateConnectionsFoldoutContent()
    {
        DrawEachStateConnection();

        DrawAddRemoveConnectionButtons();
    }

    private void DrawEachStateConnection()
    {
        //iterate over all the connections from this state to the other states its connected to
        int connectionListSize = this.connectionListArrayReference.arraySize;
        for (int i = 0; i < connectionListSize; i++)
        {
            DrawStateConnection(i);
        }
    }

    private void DrawStateConnection(int i)
    {
        DrawStateConnectionObjectData(i);
        

        switch (this.stateConnections[i].transitionHandleMethod)
        {
            case TransitionHandleMethod.handledByAgent:
                return;
            case TransitionHandleMethod.valueReferenceComparison:
                bool transitionConditionsListFoldoutRetracted = !DrawTransitionConditionsListFoldout(i);
                if (transitionConditionsListFoldoutRetracted) return;

                DrawEachTransitionCondition(i);

                DrawAddRemoveTransitionConditionButtons(i);
                break;
            case TransitionHandleMethod.handledThroughInterfaceComponent:
                DrawInspectorForHandledThroughInterfaceComp(i);
                break;
            default:
                break;
        }
    }

    private void DrawInspectorForHandledThroughInterfaceComp(int i)
    {
        EditorGUI.BeginChangeCheck();
        this.stateConnections[i].AgentPrefab =
            (GameObject)EditorGUILayout.ObjectField(this.stateConnections[i].AgentPrefab, typeof(GameObject), true);

        if (this.stateConnections[i].AgentPrefab == null)
        {
            EditorGUI.EndChangeCheck();
            return;
        }

        if (!EditorGUI.EndChangeCheck())
        {
            string[] componentNames = evaluators.Select(evaluator => evaluator.GetType().ToString()).ToArray();
            EditorGUI.BeginChangeCheck();
            this.stateConnections[i].SelectedTransitionConditionEvaluator =
                EditorGUILayout.Popup(this.stateConnections[i].SelectedTransitionConditionEvaluator, componentNames);

            if (EditorGUI.EndChangeCheck() || (this.stateConnections[i].Evaluator == null && this.stateConnections[i].transitionHandleMethod == TransitionHandleMethod.handledThroughInterfaceComponent))
            {
                this.stateConnections[i].Evaluator =
                    (Component)this.evaluators[this.stateConnections[i].SelectedTransitionConditionEvaluator];
            }

            return;
        }

        if (this.stateConnections[i].AgentPrefab != null)
        {
            GetEvaluatorsOnAgentPrefab(this.stateConnections[i].AgentPrefab);
        }
    }

    public void GetEvaluatorsOnAgentPrefab(GameObject agentPrefab)
    {
        Component[] components = agentPrefab.GetComponents<Component>();

        evaluators = components.OfType<I_TransitionEvaluator>().ToList();
    }

    private void DrawStateConnectionObjectData(int i)
    {
        object objectReferenceValue;

        EditorGUI.indentLevel = 1;

        SerializedProperty currentConnection = this.connectionList[i];
        currentConnection.isExpanded = EditorGUILayout.Foldout(currentConnection.isExpanded, "Element " + i);

        if (currentConnection.isExpanded == false)
            return;

        EditorGUI.indentLevel += 3;


        objectReferenceValue = EditorGUILayout.ObjectField("State To", this.stateConnections[i].StateTo, typeof(State), true);

        if (objectReferenceValue is State stateTo)
            if (stateTo != null)
                this.stateConnections[i].StateTo = stateTo;

        var oldMethod = this.stateConnections[i].transitionHandleMethod;
        
        this.stateConnections[i].transitionHandleMethod = (TransitionHandleMethod)EditorGUILayout.EnumPopup("Transition Handle Method", this.stateConnections[i].transitionHandleMethod);
    }

    private bool DrawTransitionConditionsListFoldout(int i)
    {
        this.conditionLists[i].isExpanded =
            EditorGUILayout.Foldout(conditionLists[i].isExpanded, "Transition Conditions");

        bool isTheConditionsListExpanded = this.conditionLists[i].isExpanded;
        return isTheConditionsListExpanded;
    }

    private void DrawEachTransitionCondition(int i)
    {
        EditorGUI.indentLevel += 2;
        int conditionListsArraySize = this.conditionLists[i].arraySize;

        for (int j = 0; j < conditionListsArraySize; j++)
        {
            DrawTransitionCondition(i, j);
        }
    }

    private void DrawTransitionCondition(int connectionIndex, int conditionOfThatConnectionIndex)
    {
        bool conditionFoldoutRetracted = DrawFoldoutForThisCondition(connectionIndex, conditionOfThatConnectionIndex);
        if (conditionFoldoutRetracted) return;

        //Get the transition condition object we are going to be drawing the data for
        TransitionConditionBase transitionCondition =
            this.stateConnections[connectionIndex].GetTransitionConditions()[conditionOfThatConnectionIndex];

        EditorGUI.indentLevel = 9;
        DrawTransitionConditionObjectData(connectionIndex, conditionOfThatConnectionIndex, transitionCondition);
    }

    private bool DrawFoldoutForThisCondition(int connectionIndex, int conditionOfThatConnectionIndex)
    {
        EditorGUI.indentLevel = 6;
        this.conditionLists[connectionIndex].GetArrayElementAtIndex(conditionOfThatConnectionIndex).isExpanded =
            EditorGUILayout.Foldout(
                this.conditionLists[connectionIndex].GetArrayElementAtIndex(conditionOfThatConnectionIndex).isExpanded,
                "Condition " + conditionOfThatConnectionIndex);

        bool conditionFoldoutRetracted = !this.conditionLists[connectionIndex].GetArrayElementAtIndex(conditionOfThatConnectionIndex).isExpanded;

        return conditionFoldoutRetracted;
    }
    private void DrawTransitionConditionObjectData(int connectionIndex, int conditionOfThatConnectionIndex,
            TransitionConditionBase transitionCondition)
    {
        DrawConditionValueReferenceField(transitionCondition);

        DrawComparisonOperatorSelector(transitionCondition);

        DrawAppropriateConditionComparand(connectionIndex, conditionOfThatConnectionIndex, transitionCondition);
    }

    private static void DrawConditionValueReferenceField(TransitionConditionBase transitionCondition)
    {
        object objectReferenceValue = EditorGUILayout.ObjectField("Value Reference",
            transitionCondition.ValueToTest,
            typeof(SuperBaseScriptableValRef), false);

        if (objectReferenceValue != null)
            transitionCondition.ValueToTest = (SuperBaseScriptableValRef)objectReferenceValue;
    }

    private static void DrawComparisonOperatorSelector(TransitionConditionBase transitionCondition)
    {
        object objectReferenceValue = EditorGUILayout.ObjectField(
            "Comparison Operator", transitionCondition.ComparisonOperator, typeof(ComparisonOperator), false);
        if (objectReferenceValue != null)
        {
            transitionCondition.ComparisonOperator = (ComparisonOperator)objectReferenceValue;
            EditorUtility.SetDirty(transitionCondition.ComparisonOperator);
        }

    }

    private void DrawAppropriateConditionComparand(int connectionIndex, int conditionOfThatConnectionIndex,
            TransitionConditionBase transitionCondition)
    {
        if (transitionCondition.ValueToTest is BoolReference && transitionCondition is BoolTransitionCondition)
        {
            transitionCondition.SetComparand(EditorGUILayout.Toggle("Comparand",
                Convert.ToBoolean(transitionCondition.GetComparand())));
        }
        else if (transitionCondition.ValueToTest is BoolReference)
        {
            transitionCondition = transitionCondition.CastToBoolCondition();
            this.stateConnections[connectionIndex]
                .SetTransitionCondition(conditionOfThatConnectionIndex, transitionCondition);
            transitionCondition.SetComparand(EditorGUILayout.Toggle("Comparand",
                Convert.ToBoolean(transitionCondition.GetComparand())));
        }
        else if (transitionCondition.ValueToTest is FloatReference &&
                 transitionCondition is FloatTransitionCondition)
        {
            transitionCondition.SetComparand(EditorGUILayout.FloatField("Comparand",
                (float)transitionCondition.GetComparand()));
        }
        else if (transitionCondition.ValueToTest is FloatReference)
        {
            transitionCondition = transitionCondition.CastToFloatCondition();
            this.stateConnections[connectionIndex]
                .SetTransitionCondition(conditionOfThatConnectionIndex, transitionCondition);
            transitionCondition.SetComparand(EditorGUILayout.FloatField("Comparand",
                (float)transitionCondition.GetComparand()));
        }
    }



    private void DrawAddRemoveTransitionConditionButtons(int i)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(this.plusIcon, GUILayout.Width(25)))
        {
            this.state.AddConditionToTransition(i);
        }

        if (GUILayout.Button(this.minusIcon, GUILayout.Width(25)))
        {
            this.state.RemoveLastConditionFromTransition(i);
        }

        EditorGUILayout.EndHorizontal();
    }
    private void DrawAddRemoveConnectionButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(this.plusIcon, GUILayout.Width(25)))
        {
            this.state.AddEmptyTransition();
            serializedObject.Update();
            this.setupHelper.SetupSerialisedProperties();
        }

        if (GUILayout.Button(this.minusIcon, GUILayout.Width(25)))
        {
            this.state.RemoveLatestTransition();
            serializedObject.Update();
            this.setupHelper.SetupSerialisedProperties();
        }

        EditorGUILayout.EndHorizontal();
    }
}

