using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Player;
    [SerializeField] float Speed;
    [SerializeField] float Rotation;
    [SerializeField] float AccepableMovementAngle;

    [SerializeField] float MoveCooldown;
    private Vector3 TargetPosition;
    private float IdleMoveTimer;

    [SerializeField] BoolReference isPlayerMoving;
    [SerializeField] BoolReference isCollecting;
    public void EnterState(State state)
    {
        isPlayerMoving.SetValue(false);
    }

    public void Behave(State state)
    {
        // we want to only do this if we are a certain range from the player
        if (Vector3.Distance(Player.transform.position, this.transform.position) < 5 && isCollecting.GetValue())
        {
            IdleMoveTimer -= Time.deltaTime;
            // get a direction
            if (!isPlayerMoving.GetValue() && IdleMoveTimer <= 0f)
            {
                Vector3 rndDirection = Random.insideUnitSphere;
                rndDirection.y = 0;
                TargetPosition = Player.transform.position + rndDirection.normalized * Random.Range(0.5f, 3.0f);

                //start timer after getting a direction
                IdleMoveTimer = MoveCooldown;
            }

            //set the direction we want to go
            Vector3 direction = (TargetPosition - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);

            float angleToTarget = Vector3.Angle(transform.forward, direction);

            if (angleToTarget > AccepableMovementAngle)
            {
                //rotate while we are not at the right angle
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
            }
            else if (distanceToTarget > 0.02f)
            {
                //move when we are in the right angle and start timer
                this.GetComponent<NavMeshAgent>().SetDestination(TargetPosition);
            }
        }
    }

    public void ExitState(State state)
    {

    }

    public bool EvaluateTransition(int connectionIndex)
    {
        if (isPlayerMoving.GetValue())
        {
            return true;
        }
        return false;
    }
}
