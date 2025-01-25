using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FSMCloner
{
    //Attributes
    //The entrypoint of the cloned finite state machine initialized to be null
    private FSMStateBase newFsmEntry = null;
    public FSMStateBase NewFsmEntry => this.newFsmEntry;

    //the searcher here is used like an iterator to traverse the fsm. this way the FSMCloner can act as a visitor and perform part of the cloning process
    //as the searcher is traversing the graph
    //The oldToNewNodes dictionary is used after the nodes have all been cloned in order to link up all the newly cloned nodes
    //in the same structure as all the old nodes
    private Dictionary<FSMStateBase, FSMStateBase> oldToNewNodes = new Dictionary<FSMStateBase, FSMStateBase>();

    public List<FSMStateBase> GetStatesList()
    {
        return this.oldToNewNodes.Values.ToList();
    }


    public void CloneGraph(List<FSMStateBase> states, GameObject owningObject)
    {
        foreach (FSMStateBase state in states)
        {
            ConstructOldToNewMap(state, owningObject);
        }
        
        LinkUpClonedNodes();

        this.newFsmEntry = this.oldToNewNodes[states[0]];
        
        #if UNITY_EDITOR
        EditorUtility.SetDirty(newFsmEntry);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        #endif
    }

    
    private void ConstructOldToNewMap(FSMStateBase current, GameObject owningObject)
    {
        if (this.oldToNewNodes.ContainsKey(current))
        {
            return;
        }
        
        //if we are seeing a new node then we can clone it and add it to the oldToNewNodes map
        FSMStateBase copy = current.Clone(owningObject);
        this.oldToNewNodes.Add(current, copy);
    }
    
    private void LinkUpClonedNodes()
    {
        foreach (FSMStateBase state in this.oldToNewNodes.Values)
        {
            switch (state)
            {
                case EntryState entry:
                    entry.stateConnection.StateTo = (State)this.oldToNewNodes[entry.stateConnection.StateTo];
                    break;
                case State normalState:
                    List<StateConnection> connections = normalState.GetStateConnections();
                    
                    for (int i = 0; i < connections.Count; i++)
                    {
                        connections[i].StateTo = (State)this.oldToNewNodes[connections[i].StateTo];
                    }
                    break;
            }
            
        }
    }
}
