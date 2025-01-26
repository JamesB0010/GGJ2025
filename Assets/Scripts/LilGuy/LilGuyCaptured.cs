using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LilGuyCaptured : MonoBehaviour
{
    [SerializeField] Transform visualTransform;
    [SerializeField] Animator bodyAnimator;
    [SerializeField] Collider bodyCollider;
    [SerializeField] SoapBubble containerBubble;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float upwardSpeed;

    private Vector3 rotationDirection;

    public void Enter(State state)
    {
        this.rotationDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; 
        containerBubble  = transform.parent.GetComponent<SoapBubble>();
        bodyAnimator.enabled = false;
        bodyCollider.enabled = false;
    }
    public void Behave(State state)
    {
        visualTransform.Rotate(this.rotationDirection, this.rotationSpeed * Time.deltaTime);
        visualTransform.position = containerBubble.transform.position;
    }
}
