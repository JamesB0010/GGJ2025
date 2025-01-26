using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectBubble : MonoBehaviour, I_TransitionEvaluator
{
    [SerializeField] GameObject Target;
    [SerializeField] float Rotation;
    [SerializeField] float AccepableMovementAngle;
    [SerializeField] Vector3 TargetFloorPosition;
    [SerializeField] private GameObject explosionParticle;

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
        bool inPosToCollect = this.transform.position.x == Target.transform.position.x && this.transform.position.z == Target.transform.position.z;
        if (inPosToCollect)
        {
            // Hook up here to the Quest Manager
            ChecklistEntity ce = Target.GetComponentInChildren<ChecklistEntity>();
            if(ce != null){
                Debug.Log($"Successfully extracted checklist entity from {Target.name}");
                ce.OnCollect();
                Destroy(this.Target.gameObject);
            }
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
