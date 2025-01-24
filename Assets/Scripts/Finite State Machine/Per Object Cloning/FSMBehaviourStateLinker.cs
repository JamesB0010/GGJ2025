using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is used to inform all the nodes (states) in a finite state machine graph of the agent game object the state machine belongs to
//this way the states know where to send the message when they want to exhibit behaviour
public class FSMBehaviourStateLinker
{
    //Attributes
    //the owning agent
    private FSMBehaviour agent;

    //the list of states which make up the state machine
    public List<FSMStateBase> states;

    //methods 
    public FSMBehaviourStateLinker(FSMBehaviour agent, List<FSMStateBase> states)
    {
        this.agent = agent;
        this.states = states;
    }

    public void Link()
    {
        for (int i = 0; i < states.Count; i++)
        {
            var linkableState = this.states[i] as State;
            if(linkableState != null)
                linkableState.SetLinkedAgent(this.agent);
        }
    }
}
