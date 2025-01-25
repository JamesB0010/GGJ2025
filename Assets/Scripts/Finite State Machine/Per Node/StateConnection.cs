using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A State Connection is a class used in the State Class (composition)
//this class holds the state connection from the parent state to 1 other state
//it also holds a list of conditions which will allow us to change from the parent state into the stateTo state
//This class is able to evaluate all the transitions and return if a transition is necessary or not

//Following feedback from carlo added transitionHandledByAgent bool to allow the agent to handle the transition

public enum TransitionHandleMethod
{
    handledByAgent,
    valueReferenceComparison,
    
    //this is for the new unity event driven system
    handledThroughInterfaceComponent,
    none
}
[Serializable]
public class StateConnection
{
    //Attributes
    [SerializeField]
    [Tooltip("The State in the FSM this connection is connected to")]
    private State stateTo;

    [SerializeField] public TransitionHandleMethod transitionHandleMethod;

    private Component evaluator;
    public Component Evaluator
    {
        get => this.evaluator;
        set
        {
            if(value is I_TransitionEvaluator)
                this.evaluator = (Component)value;
        }
    }

    [HideInInspector] public int SelectedTransitionConditionEvaluator;

    public GameObject AgentPrefab;
    
    [Tooltip("The conditions to get to the state this connection points to")]
    [SerializeReference]
    private List<TransitionConditionBase> transitionConditions = new List<TransitionConditionBase>();

    public void SetTransitionCondition(int index, TransitionConditionBase condition)
    {
        this.transitionConditions[index] = condition;
    }

    //Methods
    public StateConnection()
    {
        this.transitionConditions.Add(new FloatTransitionCondition());
    }

    //Logic
    public bool Evaluate(int connectionIndex, State stateAttachedTo, FSMBehaviour agent)
    {
        switch (this.transitionHandleMethod)
        {
            case TransitionHandleMethod.handledByAgent:
                return agent.EvaluateTransition(stateAttachedTo, this.stateTo);
            case TransitionHandleMethod.valueReferenceComparison:
                bool transition = true;
                foreach (TransitionConditionBase condition in this.transitionConditions)
                {
                    transition &= condition.Evaluate();
                }

                return transition;

            case TransitionHandleMethod.handledThroughInterfaceComponent:
                return ((I_TransitionEvaluator)this.Evaluator).EvaluateTransition(connectionIndex);
            default:
                return false;
        }
    }

    //Getter
    public State StateTo
    {
        get
        {
            return this.stateTo;
        }
        set
        {
            this.stateTo = value;
        }
    }

    public void AddNewCondition()
    {
        this.transitionConditions.Add(new FloatTransitionCondition());
    }

    public void RemoveLatestCondition()
    {
        this.transitionConditions.RemoveAt(this.transitionConditions.Count - 1);
    }

    public List<TransitionConditionBase> GetTransitionConditions()
    {
        List<TransitionConditionBase> conditions = new List<TransitionConditionBase>();
        for (int i = 0; i < this.transitionConditions.Count; i++)
        {
            conditions.Add(this.transitionConditions[i]);
        }

        return conditions;
    }

    public object Clone(GameObject owningObject)
    {
        if (this.stateTo.StateName == "Mouse Idle")
        {
            Debug.Log("this da one");
        }
        StateConnection connection = new StateConnection();
        connection.stateTo = this.stateTo;
        connection.transitionConditions = this.transitionConditions;
        connection.transitionHandleMethod = this.transitionHandleMethod;
        connection.AgentPrefab = this.AgentPrefab;
        connection.Evaluator = this.Evaluator;

        if(connection.transitionHandleMethod == TransitionHandleMethod.handledThroughInterfaceComponent)
            BindTransitionConditionMethodToInstantiatedPrefab(owningObject, connection);

        return connection;
    }

    private void BindTransitionConditionMethodToInstantiatedPrefab(GameObject owningObject,
        StateConnection connection)
    {
        if (connection.transitionHandleMethod == TransitionHandleMethod.handledThroughInterfaceComponent)
        {
            Component[] components = connection.AgentPrefab.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                I_TransitionEvaluator cast = components[i] as I_TransitionEvaluator;
                if (cast == null)
                    continue;

                if ((I_TransitionEvaluator)components[i] == (I_TransitionEvaluator)connection.Evaluator)
                {
                    connection.Evaluator = owningObject.GetComponentAtIndex<Component>(i);
                    return;
                }
            }
        }
    }
}