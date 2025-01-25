using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//The state manager is the 1 of 2 components attached to any agent with a state machine
//The state manager holds the reference to the state machine and drives its updates

//the other component is a user defined component which has methods for responding to when
//a state updates
public class FSMMonoComponent : MonoBehaviour
{
    [SerializeField]
    private FiniteStateMachine finiteStateMachine;

    public FiniteStateMachine FiniteStateMachine
    {
        get => this.finiteStateMachine;

        set => this.finiteStateMachine = value;
    }

    private void Start()
    {
        this.finiteStateMachine = this.finiteStateMachine.Clone(this.gameObject);
        LinkFSMToGameObject();
        this.finiteStateMachine.OnStart();
    }


    private void LinkFSMToGameObject()
    {
        if (TryGetComponent(out FSMBehaviour behaviour))
        {
            FSMBehaviourStateLinker linker = new FSMBehaviourStateLinker(behaviour, this.finiteStateMachine.States);
            linker.Link();
        }
    }
    
    private void Update()
    {
        this.finiteStateMachine.OnUpdate();
    }

    public void TransitionActiveState(int connectionIndex = 0)
    {
        this.finiteStateMachine.ActiveState.Transition(connectionIndex);
    }
}