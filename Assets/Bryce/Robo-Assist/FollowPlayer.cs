using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Player;
    [SerializeField] float Speed;
    [SerializeField] float Rotation;
    [SerializeField] float StopDistance;
    [SerializeField] float AccepableMovementAngle;

    [SerializeField] BoolReference isMoving;
    [SerializeField] BoolReference isCollecting;

    CharacterController controller;

    private void Start()
    {
        controller = Player.GetComponent<CharacterController>();
    }

    public void EnterState(State state)
    {

    }

    public void Behave(State state)
    {
        if (isCollecting.GetValue())
        {
            state.Transition(1);
        }
        // we want to only do this if we are a certain range from the player
        if (Vector3.Distance(Player.transform.position, this.transform.position) > 3 && !isCollecting.GetValue())
        {
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);

            float angleToTarget = Vector3.Angle(transform.forward, direction);

            if (angleToTarget > AccepableMovementAngle)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
            }
            else if (distanceToTarget > StopDistance)
            {
                this.GetComponent<NavMeshAgent>().SetDestination(Player.transform.position);
            }
        }
    } 

    public void ExitState(State state)
    {

    }

    public bool EvaluateTransition(int connectionIndex)
    {
        if (!isMoving.GetValue())
        {
            return true;
        }
        return false;
    }
}
