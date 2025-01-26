using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class roboAssistAi : FSMBehaviour
{
    [SerializeField] private State idle, follow, collect;
    
    [SerializeField] GameObject Player;
        [SerializeField] float Speed;
        [SerializeField] float Rotation;
        [SerializeField] float AccepableMovementAngle;
    
        [SerializeField] float MoveCooldown;
        private Vector3 TargetPosition;
        private float IdleMoveTimer;
    
        [SerializeField] BoolReference isPlayerMoving;
        [SerializeField] BoolReference isCollecting;
        
        
        
        [SerializeField] GameObject Player_Follow;
        [SerializeField] float Follow_Speed;
        [SerializeField] float Follow_Rotation;
        [SerializeField] float StopDistance;
        [SerializeField] float Follow_AccepableMovementAngle;

        [SerializeField] BoolReference isMoving;
        [SerializeField] BoolReference Follow_isCollecting;

        CharacterController controller; 
        
        
        [SerializeField] GameObject Target;
        [SerializeField] float Collect_Rotation;
        [SerializeField] float Collect_AccepableMovementAngle;
        [SerializeField] Vector3 Collect_TargetFloorPosition;
        [SerializeField] private GameObject explosionParticle;
        [SerializeField] private UnityEvent robotExplosion;

        [SerializeField] BoolReference Collection_isCollecting;

        CharacterController Collection_controller; 

        
        public void InitialiseTarget(GameObject InTarget)
        {
            Target = InTarget;
        }

        private void Start()
            {
                controller = Player_Follow.GetComponent<CharacterController>();
            }
        
        public override void EnterState(State state)
        {
            if (state.IsInstanceOf(this.idle))
            {
                isCollecting.SetValue(false);
            }
        }

        public override void Behave(State state)
        {
            if (state.IsInstanceOf(this.idle))
            {
                if (isCollecting.GetValue())
                {
                    state.Transition(1);
                }

                // we want to only do this if we are a certain range from the player
                if (Vector3.Distance(Player.transform.position, this.transform.position) < 5)
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
                        transform.rotation =
                            Quaternion.Slerp(transform.rotation, targetRotation, Rotation * Time.deltaTime);
                    }
                    else if (distanceToTarget > 0.02f)
                    {
                        //move when we are in the right angle and start timer
                        this.GetComponent<NavMeshAgent>().SetDestination(TargetPosition);
                    }
                }
            }

            if (state.IsInstanceOf(this.follow))
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

            if (state.IsInstanceOf(this.collect))
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
        }

        public override void ExitState(State state)
        {
            if (state.IsInstanceOf(this.collect))
            {
                Collection_isCollecting.SetValue(false);
            }
        }

        public override bool EvaluateTransition(State current, State to)
        {
            if (current.IsInstanceOf(idle) || current.IsInstanceOf(this.follow))
            {
                if (isPlayerMoving.GetValue() && !isCollecting.GetValue())
                {
                    return true;
                }
            }

            return false;
        }
}
