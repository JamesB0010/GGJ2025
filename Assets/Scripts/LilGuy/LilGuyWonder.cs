using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LilGuyWonder : MonoBehaviour
{
    [SerializeField] private float timeBetweenNewWonderPos;

    [SerializeField] private float speed;

    private Vector3 targetDirection;

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

        ApplySteering();
        this.countdownToNewWonderPos -= Time.deltaTime;
    }

    private void ApplySteering()
    {
        Vector3 velocity = this.targetDirection * this.speed * Time.deltaTime;
        transform.Translate(velocity);
    }

    public void ExitState(State state)
    {
        this.countdownToNewWonderPos = timeBetweenNewWonderPos;
    }

    public void RandomizeTargetDirection()
    {
        this.targetDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;
    }
}
