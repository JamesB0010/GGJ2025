using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateConnectionsWrapper : IEnumerable<StateConnection>
{
    [SerializeField] public List<StateConnection> stateConnections = new();
    public int Count => this.stateConnections.Count;

    public IEnumerator<StateConnection> GetEnumerator()
    {
        return this.stateConnections.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void Add(StateConnection stateConnection)
    {
        this.stateConnections.Add(stateConnection);
    }

    public void RemoveAt(int count)
    {
        this.stateConnections.RemoveAt(count);
    }

    public bool Remove(StateConnection connection)
    {
        return this.stateConnections.Remove(connection);
    }

    public StateConnection this[int transitionIndex] => this.stateConnections[transitionIndex];

    public void ForEach(Action<StateConnection> callback)
    {
        foreach (var connection in this.stateConnections)
        {
            callback(connection);
        }
    }
}
