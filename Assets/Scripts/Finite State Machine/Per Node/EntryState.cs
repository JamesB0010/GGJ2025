using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntryState : FSMStateBase
{
    protected string stateName = "Entry State";


    [TextArea] [SerializeField] protected string description =
        "This state is the entry of the FSM. It has 1 connection and will automatically transition to that connection";

    public string Description => this.description;

    public StateConnection stateConnection = new StateConnection();

    public override FSMStateBase Clone(GameObject owningObject)
    {
        //Create a new instance of a state
        EntryState state = ScriptableObject.CreateInstance<EntryState>();
        state.stateName = this.stateName;
        state.description = this.description;
        state.guid = this.guid;
        state.graphEditorPosition = this.graphEditorPosition;

        //Copy over data by value
        state.name = this.stateName + "_clone";

        state.stateConnection = (StateConnection)this.stateConnection.Clone(owningObject);

        return state;
    }
}
