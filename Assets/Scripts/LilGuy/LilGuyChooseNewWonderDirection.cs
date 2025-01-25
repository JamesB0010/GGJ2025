using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LilGuyChooseNewWonderDirection : MonoBehaviour
{
    private LilGuyWonder lilGuyWonder;

    private void Start()
    {
        this.lilGuyWonder = GetComponent<LilGuyWonder>();
    }

    public void EnterState(State state)
    {
        Debug.Log("Choose a new wonder state");
        this.lilGuyWonder.RandomizeTargetDirection();
        
        state.Transition();
    }
    
}