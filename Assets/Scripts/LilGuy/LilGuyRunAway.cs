using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilGuyRunAway : MonoBehaviour
{
    private LilGuySharedData sharedData;

    private void Start()
    {
        this.sharedData = GetComponent<LilGuySharedData>();
    }

    public void Behave(State state)
    {
        
    }
    private void CalculateRunAwayForce()
    {
    }
}
