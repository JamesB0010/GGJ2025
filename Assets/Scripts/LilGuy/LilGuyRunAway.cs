using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilGuyRunAway : MonoBehaviour, I_TransitionEvaluator
{
    private LilGuySharedData sharedData;

    private LilGuyWander lilGuyWander;

    

    public void Behave(State state)
    {
        
    }
    

    public bool EvaluateTransition(int connectionIndex)
    {
        return false;
    }
}
