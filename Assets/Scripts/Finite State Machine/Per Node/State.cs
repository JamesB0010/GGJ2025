using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

//Each state is a scriptable object which can be created as an asset
//A state has a list of connected states 
//each state has a behave method where it will send a message to the owning agent for them to respond
[Serializable]
public class State : FSMStateBase
{
    
    private string stateName = "State";

    public string StateName
    {
        get => this.stateName;
        set
        {
            this.stateName = value;
            base.name = this.StateName;
        }
    }
    
[TextArea] [SerializeField] protected string description = "";

    public string Description
    {
        get => this.description;
        set => this.description = value;
    }

    //these are for linking behaviour between state and user defined scripts
    public UnityEvent<State> EnterStateEvent = new();
    public UnityEvent<State> BehaveEvent = new();
    public UnityEvent<State> ExitStateEvent = new();

    private List<Action<State>> enterStateCallbacks = new();
    private List<Action<State>> behaveCallbacks = new(); 
    private List<Action<State>> ExitStateCallbacks = new();
    
    public bool active { get; set; }
    //Attributes
    //The game object agent which owns the fsm this state is part of. The actual behaviours of each state will be part of components attached to that object
    private FSMBehaviour agent;

    [SerializeField] [Tooltip("The other States connected to this state")]
    public StateConnectionsWrapper stateConnections = new StateConnectionsWrapper();

    //Behavioural methods

    public void EnterState()
    {
        this.active = true;
        this.enterStateCallbacks.ForEach(cb => cb.Invoke(this));
        this.agent?.EnterState(this);
    }
    public void Behave()
    {
        this.behaveCallbacks.ForEach(cb => cb.Invoke(this));
        this.agent?.Behave(this);
    }

    public void SetLinkedAgent(FSMBehaviour agentObject)
    {
        this.agent = agentObject;
    }

    public State TestTransitions()
    {
        for (int i = 0; i < this.stateConnections.Count; i++)
        {
            if (this.stateConnections[i].Evaluate(i, this, this.agent))
            {
                return this.stateConnections[i].StateTo;
            }
        }
        
        return this;
    }


    //Getters
    public List<State> GetConnectedStates()
    {
        List<State> states = new List<State>();
        foreach (StateConnection connection in this.stateConnections)
        {
            states.Add(connection.StateTo);
        }

        return states;
    }

    public List<StateConnection> GetStateConnections()
    {
        return this.stateConnections.stateConnections;
    }

    public void AddEmptyTransition()
    {
        this.stateConnections.Add(new StateConnection());
    }

    public void RemoveLatestTransition()
    {
        this.stateConnections.RemoveAt(this.stateConnections.Count - 1);
    }

    public void AddConditionToTransition(int transitionIndex)
    {
        this.stateConnections[transitionIndex].AddNewCondition();
    }

    public void RemoveLastConditionFromTransition(int transitionIndex)
    {
        this.stateConnections[transitionIndex].RemoveLatestCondition();
    }

    //Cloning
    public override FSMStateBase Clone(GameObject owningObject)
    {
        //Create a new instance of a state
        this.StateName = this.name;
        
        State state = ScriptableObject.CreateInstance<State>();
        state.stateName = this.stateName;
        state.description = this.description;
        state.guid = this.guid;
        state.graphEditorPosition = this.graphEditorPosition;

        //Copy over data by value
        state.name = this.stateName + "_clone";

        if (state.stateName == "Mouse Idle")
        {
            Debug.Log("hi");
        }

        //Deep copy the state connections
        DeepCopyStateConnections(state, owningObject);

        state.EnterStateEvent = this.EnterStateEvent;
        state.BehaveEvent = this.BehaveEvent;
        state.ExitStateEvent = this.ExitStateEvent;

        state.ResolveUnityEventDependencies(owningObject);
        
        return state;
    }

    private void ResolveUnityEventDependencies(GameObject owningObject)
    {
        this.enterStateCallbacks.AddRange(this.ResolveStateEvent(this.EnterStateEvent, owningObject));
        this.behaveCallbacks.AddRange(this.ResolveStateEvent(this.BehaveEvent, owningObject));
        this.ExitStateCallbacks.AddRange(this.ResolveStateEvent(this.ExitStateEvent, owningObject));
    }

    private List<Action<State>> ResolveStateEvent(UnityEvent<State> stateEvent, GameObject owningObject)
    {
        List<Action<State>> callbackList = new List<Action<State>>();
        for (int i = 0; i < stateEvent.GetPersistentEventCount(); i++)
        {
            Object oldTargetComponent = stateEvent.GetPersistentTarget(i);
            string methodName = stateEvent.GetPersistentMethodName(i);

            Action<State> cb = this.ResolveEventDelegateForOwningObj(owningObject, oldTargetComponent, methodName);

            if (cb != null)
                callbackList.Add(cb);
        }

        return callbackList;
    }

    private Action<State> ResolveEventDelegateForOwningObj(GameObject owningObject, Object oldTargetComponent,
        string methodName)
    {
        MethodInfo method = oldTargetComponent.GetType().GetMethod(methodName);
        
        //so we know this method is going to exist on one of the 
        if (method != null)
        {
            var componentType = oldTargetComponent.GetType();
            Action<State> action = (Action<State>)Delegate.CreateDelegate(typeof(Action<State>), owningObject.GetComponent(componentType), method);
            return action;
        }
        
        Debug.LogWarning($"Method {methodName} not found on target {owningObject.name}");
        return null;
    }

    private void DeepCopyStateConnections(State state, GameObject owningObject)
    {
        state.stateConnections = new StateConnectionsWrapper();
        for (int i = 0; i < this.stateConnections.Count; i++)
        {
            StateConnection clonedConnection = (StateConnection)this.stateConnections[i].Clone(owningObject);
            state.stateConnections.Add(clonedConnection);
        }
    }

    public bool IsInstanceOf(State other)
    {
        return this.StateName == other.StateName;
    }

    public void Transition(int connectionIndex = 0)
    {
        if(base.Parent.ActiveState == this)
            base.Parent.ActiveState = this.stateConnections[connectionIndex].StateTo;
    }

    public void ExitState()
    {
        this.agent?.ExitState(this);
        this.ExitStateCallbacks.ForEach(cb => cb(this));
        this.active = false;
    }

    public void AddToEnterStateCallbackList(Action<State> enteredStateCallback)
    {
        this.enterStateCallbacks.Add(enteredStateCallback);
    }

    public void AddToExitStateCallbackList(Action<State> exitedStateCallback)
    {
        this.ExitStateCallbacks.Add(exitedStateCallback);
    }
}