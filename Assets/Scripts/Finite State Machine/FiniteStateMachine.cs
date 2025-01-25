using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class FiniteStateMachine : ScriptableObject
{
    private FiniteStateMachine prototype;
    public FiniteStateMachine Prototype => this.prototype;
    public event Action<FSMStateBase, FSMStateBase> stateChanged; 
    private State activeState;

    public State ActiveState
    {
        get => this.activeState;
        set
        {
            if (this.activeState != value)
            {
                this.readyToChangeState = true;
                this.newStateToChangeTo = value;
            }
        }
    }

    [FormerlySerializedAs("ReadyToChangeState")] public bool readyToChangeState;
    public State newStateToChangeTo;
    
    
    [SerializeField] private EntryState entryState = null;

    [SerializeField] private List<FSMStateBase> states = new();
    public List<FSMStateBase> States => this.states;

    public EntryState EntryState
    {
        get => this.entryState;
        set => this.entryState = value;
    }

    public void CreateEntryState()
    {
        this.entryState = ScriptableObject.CreateInstance<EntryState>();
        this.entryState.name = "Entry State";
        #if UNITY_EDITOR
        this.entryState.guid = GUID.Generate().ToString();
        #endif
        this.states.Add(this.entryState);
        this.entryState.Parent = this;

        #if UNITY_EDITOR
        if (!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(this.entryState, this);
        AssetDatabase.SaveAssets();
        #endif
    }
    
    public State CreateState()
    {
        State state = ScriptableObject.CreateInstance<State>();
        state.Parent = this;
        state.name = "State";
        #if UNITY_EDITOR
        state.guid = GUID.Generate().ToString();
        
        Undo.RecordObject(this, "FSM (Create State)");
        #endif
        this.states.Add(state);
        
        #if UNITY_EDITOR
        if(!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(state, this);
        
        Undo.RegisterCreatedObjectUndo(state, "FSM (Create State");
        AssetDatabase.SaveAssets();

        #endif
        return state;
    }

    public void DeleteState(State state)
    {
        #if UNITY_EDITOR
        Undo.RecordObject(this, "FSM (Delete Node)");
        #endif
        this.states.Remove(state);
        
        #if UNITY_EDITOR
        Undo.DestroyObjectImmediate(state);
        AssetDatabase.SaveAssets();
        #endif
    }

    public void MakeConnection(FSMStateBase start, FSMStateBase to)
    {
        StateConnection stateConnection = new StateConnection();
        stateConnection.StateTo = to as State;

        switch (start)
        {
            case EntryState entryState:
                entryState.stateConnection = stateConnection; 
                break;
            case State state:
                state.stateConnections.Add(stateConnection);
                break;
        }
    }

    public void BreakConnection(FSMStateBase start, FSMStateBase to)
    {
        switch (start)
        {
            case EntryState entryState:
                entryState.stateConnection = new StateConnection();
                break;
            case State state:
                //Get all the connections from the start state to the to state
                var connectionsToDelete = state.stateConnections.Where(connection => connection.StateTo == to);
                        
                foreach (StateConnection connection in connectionsToDelete)
                {
                    state.stateConnections.Remove(connection);
                }
                break;
        }
    }

    public void OnStart()
    {
        this.activeState = this.entryState.stateConnection.StateTo;
        this.activeState.EnterState();
    }

    public void OnUpdate()
    {
        if (this.readyToChangeState)
        {
            this.ChangeState(this.newStateToChangeTo);
        }
        State newState = activeState.TestTransitions();
        if (this.activeState != newState)
        {
            ChangeState(newState);
        }


        activeState.Behave();
    }

    private void ChangeState(State newState)
    {
        this.readyToChangeState = false;
        activeState = newState;
        //Start
        this.stateChanged?.Invoke(this.activeState, newState);

        this.activeState.ExitState();
        newState.EnterState();
    }

    public FiniteStateMachine Clone(GameObject owningObject)
    {
        FiniteStateMachine clone = ScriptableObject.CreateInstance<FiniteStateMachine>();
        
        FSMCloner cloner = new FSMCloner();
        cloner.CloneGraph(this.states, owningObject);
        clone.EntryState = (EntryState)cloner.NewFsmEntry;
        clone.states = cloner.GetStatesList();
        clone.states.ForEach(state => state.Parent = clone);
        clone.prototype = this;

        return clone;
    }
}
