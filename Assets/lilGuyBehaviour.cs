using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lilGuyBehaviour : FSMBehaviour
{
    [SerializeField] private State wonder, runAway, captured;
    
    [Header("Wander Settings")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float circleDistance = 2f;
        [SerializeField] private float circleRadius = 1f;
        [SerializeField] private float angleChange = 30f;
        [SerializeField] private float lookAheadDistance;
        [SerializeField] private float avoidenceStrength;
        [SerializeField] private float playerDetectionRange;
        [SerializeField] private LilGuySharedData sharedData;
    
        private Vector3 targetDirection;
        private float wanderAngle;
    
        [SerializeField] private Transform[] itemsToAvoid;
        
        [SerializeField] private float runAwaySpeed;
        
            [SerializeField] private float noLongerFleeRange;
            
            [SerializeField] Transform visualTransform;
                [SerializeField] Animator bodyAnimator;
                [SerializeField] Collider bodyCollider;
                [SerializeField] SoapBubble containerBubble;
                [SerializeField] private float rotationSpeed;
                [SerializeField] private float upwardSpeed;
            
                private Vector3 rotationDirection;
        
        private void Start()
            {
                wanderAngle = UnityEngine.Random.Range(0f, 360f);
                this.sharedData = GetComponent<LilGuySharedData>();
            }

        public override void EnterState(State state)
        {
            if (state.IsInstanceOf(this.captured))
            {
                this.rotationDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; 
                        containerBubble  = transform.parent.GetComponent<SoapBubble>();
                        bodyAnimator.enabled = false;
                        bodyCollider.enabled = false;
            }
        }

        public override void Behave(State state)
        {
            if (state.IsInstanceOf(this.wonder))
            {
                Vector3 velocity = this.CalculateWanderForce();
                        velocity += this.calculateAvoidenceForce(velocity);
                        this.Move(velocity);
                        this.RotateTowards(velocity.normalized);
            }

            if (state.IsInstanceOf(this.runAway))
            {
                Vector3 velocity = this.CalculateRunAwayForce();
                        velocity += this.calculateAvoidenceForce(velocity);
                        this.Move(velocity);
                        this.RotateTowards(velocity.normalized);
            }

            if (state.IsInstanceOf(this.captured))
            {
                visualTransform.Rotate(this.rotationDirection, this.rotationSpeed * Time.deltaTime);
                        visualTransform.position = containerBubble.transform.position;
            }
        }
        
        private Vector3 CalculateRunAwayForce()
            {
                Vector3 runawayDirection = (transform.position - this.sharedData.Player.position).normalized;
                Vector3 runawayVelocity = runawayDirection * this.runAwaySpeed;
                runawayVelocity.y = 0;
                return runawayVelocity;
            }
        
        private Vector3 CalculateWanderForce()
            {
                // Calculate the circle center in world space
                Vector3 circleCenter = transform.position + transform.forward * circleDistance;
        
                // Calculate displacement based on wander angle
                Vector3 displacement = new Vector3(
                    Mathf.Cos(wanderAngle * Mathf.Deg2Rad) * circleRadius,
                    0,
                    Mathf.Sin(wanderAngle * Mathf.Deg2Rad) * circleRadius
                );
        
                // Update wander angle
                wanderAngle = Mathf.Repeat(wanderAngle + UnityEngine.Random.Range(-angleChange / 2f, angleChange / 2f), 360f);
        
                // Update target direction for visualization
                targetDirection = circleCenter + displacement;
        
                Vector3 force = (circleCenter + displacement) - transform.position;
                force.y = 0;
                return force;
            }
        
        public Vector3 calculateAvoidenceForce(Vector3 velocity)
            {
                velocity = Vector3.ClampMagnitude(velocity, this.maxSpeed);
                Vector3 start = transform.position + transform.forward * maxSpeed;
                Debug.DrawLine(transform.position, start + transform.forward * this.lookAheadDistance, Color.red);
                Vector3 avoidenceForce = Vector3.zero;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, this.lookAheadDistance))
                {
                    avoidenceForce = Vector3.Reflect(transform.forward, hit.normal);
                    avoidenceForce.Normalize();
                    avoidenceForce *= this.avoidenceStrength;
                }
        
                avoidenceForce.y = 0;
                return avoidenceForce;
            }
        public void Move(Vector3 velocity)
            {
                velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
                transform.position += velocity * Time.deltaTime;
            }
        
        public void RotateTowards(Vector3 direction)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        
        private void OnDrawGizmosSelected()
            {
                // Visualize the wander circle and direction
                Gizmos.color = Color.green;
                Vector3 circleCenter = transform.position + transform.forward * circleDistance;
                Gizmos.DrawWireSphere(circleCenter, circleRadius);
            }

        public override bool EvaluateTransition(State current, State to)
        {
            if (current.IsInstanceOf(this.wonder))
            {
                return Vector3.Distance(transform.position, this.sharedData.Player.position) < this.playerDetectionRange;
            }

            if (current.IsInstanceOf(this.runAway))
            {
                return Vector3.Distance(transform.position, this.sharedData.Player.position) > this.noLongerFleeRange;
            }

            return false;
        }
}
