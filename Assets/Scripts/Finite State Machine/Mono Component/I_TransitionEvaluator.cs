using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_TransitionEvaluator
{
    public bool EvaluateTransition(int connectionIndex);
}
