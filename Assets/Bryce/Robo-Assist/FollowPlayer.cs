using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class FollowPlayer : MonoBehaviour, I_TransitionEvaluator
{
    [FormerlySerializedAs("Player")] [SerializeField] GameObject Player_Follow;
    [FormerlySerializedAs("Speed")] [SerializeField] float Follow_Speed;
    [FormerlySerializedAs("Rotation")] [SerializeField] float Follow_Rotation;
    [SerializeField] float StopDistance;
    [FormerlySerializedAs("AccepableMovementAngle")] [SerializeField] float Follow_AccepableMovementAngle;

    [SerializeField] BoolReference isMoving;
    [FormerlySerializedAs("isCollecting")] [SerializeField] BoolReference Follow_isCollecting;

    CharacterController controller;

    private void Start()
    {
        controller = Player_Follow.GetComponent<CharacterController>();
    }

    public void EnterState(State state)
    {

    }

    public void Behave(State state)
    {
        if (Follow_isCollecting.GetValue())
        {
            state.Transition(1);
        }
        // we want to only do this if we are a certain range from the player
        if (Vector3.Distance(Player_Follow.transform.position, this.transform.position) > 3)
        {
            Vector3 direction = (Player_Follow.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, Player_Follow.transform.position);

            float angleToTarget = Vector3.Angle(transform.forward, direction);

            if (angleToTarget > Follow_AccepableMovementAngle)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Follow_Rotation * Time.deltaTime);
            }
            else if (distanceToTarget > StopDistance)
            {
                this.GetComponent<NavMeshAgent>().SetDestination(Player_Follow.transform.position);
            }
        }
    } 

    public void ExitState(State state)
    {

    }

    public bool EvaluateTransition(int connectionIndex)
    {
        if (!isMoving.GetValue() && !Follow_isCollecting.GetValue())
        {
            return true;
        }
        return false;
    }
}
