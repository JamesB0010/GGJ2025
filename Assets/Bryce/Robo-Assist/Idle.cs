using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : MonoBehaviour, I_TransitionEvaluator
{
    public bool EvaluateTransition(int connectionIndex)
    {
        return false;
    }
}
