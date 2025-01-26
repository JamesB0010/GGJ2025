using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilGuyRunAway : MonoBehaviour, I_TransitionEvaluator
{
    private LilGuySharedData sharedData;

    private LilGuyWander lilGuyWander;

    [SerializeField] private float runAwaySpeed;

    [SerializeField] private float noLongerFleeRange;

    private void Start()
    {
        this.sharedData = GetComponent<LilGuySharedData>();
        this.lilGuyWander = GetComponent<LilGuyWander>();
    }

    public void Behave(State state)
    {
        Vector3 velocity = this.CalculateRunAwayForce();
        velocity += this.lilGuyWander.calculateAvoidenceForce(velocity);
        this.lilGuyWander.Move(velocity);
        this.lilGuyWander.RotateTowards(velocity.normalized);
    }
    private Vector3 CalculateRunAwayForce()
    {
        Vector3 runawayDirection = (transform.position - this.sharedData.Player.position).normalized;
        Vector3 runawayVelocity = runawayDirection * this.runAwaySpeed;
        runawayVelocity.y = 0;
        return runawayVelocity;
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        return Vector3.Distance(transform.position, this.sharedData.Player.position) > this.noLongerFleeRange;
    }
}
