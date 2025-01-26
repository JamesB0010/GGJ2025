using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CollectBubble : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Target;
    [SerializeField] float Rotation;
    [SerializeField] float AccepableMovementAngle;
    [SerializeField] Vector3 TargetFloorPosition;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private UnityEvent robotExplosion;

    [SerializeField] BoolReference isCollecting;

    CharacterController controller;

    public void InitialiseTarget(GameObject InTarget)
    {
        Target = InTarget;
    }

    public void Behave(State state)
    {
        if(NavMesh.SamplePosition(Target.transform.position, out NavMeshHit nmh, Mathf.Infinity, NavMesh.AllAreas)){
            TargetFloorPosition = nmh.position;
        }

        // we want to only do this if we are a certain range from the player
        Vector3 direction = (TargetFloorPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, TargetFloorPosition);

        float angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget > AccepableMovementAngle)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
        }
        else{
            this.GetComponent<NavMeshAgent>().SetDestination(TargetFloorPosition);
        }

        float differenceX = Target.transform.position.x - this.transform.position.x;
        float differenceY = Target.transform.position.y - this.transform.position.y;
        float threashold = 1f;
        bool inPosToCollect = differenceX <= threashold && differenceY <= threashold;
        if (inPosToCollect)
        {
            Destroy(this.Target.gameObject);
            this.robotExplosion?.Invoke();
            state.Transition(0);
        }

    }
    public void ExitState(State state)
    {
        isCollecting.SetValue(false);
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        return false;
    }
}
