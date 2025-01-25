using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectBubble : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Target;
    [SerializeField] float Speed;
    [SerializeField] float Rotation;
    [SerializeField] float StopDistance;
    [SerializeField] float AccepableMovementAngle;

    [SerializeField] BoolReference isCollecting;

    CharacterController controller;
    public void EnterState(State state)
    {

    }

    public void Behave(State state)
    {
        // we want to only do this if we are a certain range from the player
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        float angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget > AccepableMovementAngle)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
        }
        else if (distanceToTarget > StopDistance)
        {
            this.GetComponent<NavMeshAgent>().SetDestination(Target.transform.position);
        }
    }

    public void ExitState(State state)
    {

    }

    public bool EvaluateTransition(int connectionIndex)
    {
        return false;
    }
}
