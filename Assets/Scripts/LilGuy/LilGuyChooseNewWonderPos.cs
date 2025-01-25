using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LilGuyChooseNewWonderPos : MonoBehaviour
{
    public void EnterState(State state)
    {
        Debug.Log("Choose a new wonder state");
        
        state.Transition();
    }
    
}