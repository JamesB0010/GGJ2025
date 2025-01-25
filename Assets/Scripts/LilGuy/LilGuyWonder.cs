using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilGuyWonder : MonoBehaviour
{
    [SerializeField] private float timeBetweenNewWonderPos;

    private float countdownToNewWonderPos;

    private void Start()
    {
        this.countdownToNewWonderPos = this.timeBetweenNewWonderPos;
    }

    public void Behave(State state)
    {
        if (countdownToNewWonderPos <= 0)
        {
            state.Transition();
            
            return;
        }

        this.countdownToNewWonderPos -= Time.deltaTime;
    }

    public void ExitState(State state)
    {
        this.countdownToNewWonderPos = timeBetweenNewWonderPos;
    }
}
