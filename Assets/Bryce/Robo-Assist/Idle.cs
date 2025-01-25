using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Player;
    [SerializeField] float Speed;
    [SerializeField] float Rotation;
    [SerializeField] float AccepableMovementAngle;

    [SerializeField] float MoveCooldown;
    private Vector3 TargetPosition;
    private float IdleMoveTimer;

    [SerializeField] BoolReference isMoving;
    public void EnterState(State state)
    {
        isMoving.SetValue(false);
    }

    public void Behave(State state)
    {
        IdleMoveTimer -= Time.deltaTime;

        
        if (Vector3.Distance(Player.transform.position, this.transform.position) > 3.0f)
        {
            state.Transition(0);
        }

        if (!isMoving.GetValue() && IdleMoveTimer <= 0f)
        {
            Vector3 rndDirection = Random.insideUnitSphere;
            rndDirection.y = 0;
            TargetPosition = Player.transform.position + rndDirection.normalized * Random.Range(0.5f, 3.0f);

            IdleMoveTimer = MoveCooldown;
        }

        Vector3 direction = (TargetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, Player.transform.position);

        float angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget > AccepableMovementAngle)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
        }
        else if (distanceToTarget > 0.2f)
        {
            transform.position += direction * Speed * Time.deltaTime;
        }
    }

    public void ExitState(State state)
    {
        isMoving.SetValue(true);
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        if (isMoving.GetValue())
        {
            return true;
        }
        return false;
    }
}
