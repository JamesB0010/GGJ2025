using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class FSMStateBase : ScriptableObject
{
    [HideInInspector] public string guid;

    [SerializeField] private FiniteStateMachine parent;
    public FiniteStateMachine Parent
    {
        get => this.parent;

        set => this.parent = value;
    }
    
    [HideInInspector] public Vector2 graphEditorPosition = new Vector2();
    


    public abstract FSMStateBase Clone(GameObject owningObject);
}