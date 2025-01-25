using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Player;
    [SerializeField] float Speed;
    [SerializeField] float Rotation;
    [SerializeField] float StopDistance;
    [SerializeField] float AccepableMovementAngle;

    private bool isMovingLocal;

    [SerializeField] BoolReference isMoving;

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
            transform.position += direction * Speed * Time.deltaTime;
        }

        if (controller.velocity.magnitude > 0)
        {
            isMovingLocal = true;
        }
        else
        {
            isMovingLocal = false;
        }
    }

    public void ExitState(State state)
    {

    }

    public bool EvaluateTransition(int connectionIndex)
    {
        if (!isMovingLocal)
        {
            isMoving.SetValue(false);
            return true;
        }
        return false;
    }
}
