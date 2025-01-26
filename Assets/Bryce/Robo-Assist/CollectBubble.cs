using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CollectBubble : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Target;
    [FormerlySerializedAs("Rotation")] [SerializeField] float Collect_Rotation;
    [FormerlySerializedAs("AccepableMovementAngle")] [SerializeField] float Collect_AccepableMovementAngle;
    [FormerlySerializedAs("TargetFloorPosition")] [SerializeField] Vector3 Collect_TargetFloorPosition;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private UnityEvent robotExplosion;

    [FormerlySerializedAs("isCollecting")] [SerializeField] BoolReference Collection_isCollecting;

    CharacterController Collection_controller;

    public void InitialiseTarget(GameObject InTarget)
    {
        Target = InTarget;
    }

    public void Behave(State state)
    {
        if(NavMesh.SamplePosition(Target.transform.position, out NavMeshHit nmh, Mathf.Infinity, NavMesh.AllAreas)){
            Collect_TargetFloorPosition = nmh.position;
        }

        // we want to only do this if we are a certain range from the player
        Vector3 direction = (Collect_TargetFloorPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, Collect_TargetFloorPosition);

        float angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget > Collect_AccepableMovementAngle)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Collect_Rotation * Time.deltaTime);
        }
        else{
            this.GetComponent<NavMeshAgent>().SetDestination(Collect_TargetFloorPosition);
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
            Instantiate(this.explosionParticle, Collect_TargetFloorPosition + (Vector3.up * 4.9f), Quaternion.identity);
            state.Transition(0);
        }

    }
    public void ExitState(State state)
    {
        Collection_isCollecting.SetValue(false);
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        return false;
    }
}
