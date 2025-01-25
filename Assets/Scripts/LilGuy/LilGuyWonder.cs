using System;
using EvolveGames;
using UnityEngine;

public class LilGuyWander : MonoBehaviour, I_TransitionEvaluator
{
    [Header("Wander Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float circleDistance = 2f;
    [SerializeField] private float circleRadius = 1f;
    [SerializeField] private float angleChange = 30f;
    [SerializeField] private float lookAheadDistance;
    [SerializeField] private float avoidenceStrength;
    [SerializeField] private float playerDetectionRange;
    private LilGuySharedData sharedData;

    private Vector3 targetDirection;
    private float wanderAngle;

    [SerializeField] private Transform[] itemsToAvoid;

    private void Start()
    {
        wanderAngle = UnityEngine.Random.Range(0f, 360f);
    }

    public void Behave(State state)
    {
        Vector3 velocity = this.CalculateWanderForce();
        velocity += this.calculateAvoidenceForce(velocity);
        this.Move(velocity);
        this.RotateTowards(velocity.normalized);
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

        return (circleCenter + displacement) - transform.position;
    }

    private Vector3 calculateAvoidenceForce(Vector3 velocity)
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

        return avoidenceForce;
    }

    private void Move(Vector3 velocity)
    {
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the wander circle and direction
        Gizmos.color = Color.green;
        Vector3 circleCenter = transform.position + transform.forward * circleDistance;
        Gizmos.DrawWireSphere(circleCenter, circleRadius);
    }

    public bool EvaluateTransition(int connectionIndex)
    {
        return Vector3.Distance(transform.position, this.sharedData.Player.position) < this.playerDetectionRange;
    }
}
