using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Player;

    public void Behave(State state)
    {
        
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        throw new System.NotImplementedException();
    }
}
