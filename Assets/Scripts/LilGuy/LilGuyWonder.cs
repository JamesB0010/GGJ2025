using System;
using UnityEngine;

public class LilGuyWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float circleDistance = 2f;
    [SerializeField] private float circleRadius = 1f;
    [SerializeField] private float angleChange = 30f;

    private Vector3 targetDirection;
    private float wanderAngle;

    private void Start()
    {
        wanderAngle = UnityEngine.Random.Range(0f, 360f);
    }

    private void Update()
    {
        Wander();
    }

    private void Wander()
    {
        // Calculate wander force and update target direction
        Vector3 wanderForce = CalculateWanderForce();
        targetDirection = wanderForce.normalized;

        // Move and rotate the object
        Move(targetDirection);
        RotateTowards(targetDirection);
    }

    private Vector3 CalculateWanderForce()
    {
        // Calculate the circle center in front of the object
        Vector3 circleCenter = transform.forward * circleDistance;

        // Calculate displacement from the circle center
        Vector3 displacement = new Vector3(0, 0, -circleRadius);
        displacement = SetAngle(displacement);

        // Update wander angle
        wanderAngle = (wanderAngle + UnityEngine.Random.Range(-angleChange / 2f, angleChange / 2f)) % 360;

        return circleCenter + displacement;
    }

    private Vector3 SetAngle(Vector3 vector)
    {
        float length = vector.magnitude;
        return new Vector3(
            Mathf.Cos(wanderAngle * Mathf.Deg2Rad) * length,
            0,
            Mathf.Sin(wanderAngle * Mathf.Deg2Rad) * length
        );
    }

    private void Move(Vector3 direction)
    {
        Vector3 velocity = direction * (speed * Time.deltaTime);
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity;
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

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + targetDirection);
    }
}
