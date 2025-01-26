using System;
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

        Vector3 target = Target.transform.position;
        target.y = transform.position.y;
        if (Vector3.Distance(transform.position, target) <= 2f)
        {
            // Hook up here to the Quest Manager
            ChecklistEntity ce = Target.GetComponentInChildren<ChecklistEntity>();
            if(ce != null){
                Debug.Log($"Successfully extracted checklist entity from {Target.name}");
                ce.OnCollect();
            }
            Destroy(this.Target.gameObject);
            this.robotExplosion?.Invoke();
            Instantiate(this.explosionParticle, TargetFloorPosition + (Vector3.up * 4.9f), Quaternion.identity);
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
