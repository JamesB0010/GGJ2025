using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoapBubble : MonoBehaviour
{
    [SerializeField] bool isInMovement = false; // turned on after being fired
    [SerializeField] bool isFull; // turned on after colliding with a damp creature
    [SerializeField] SphereCollider bubbleCollider;

    [Header("Movement Parameters")]
    Vector3 movementDirection = new Vector3(0,0,0);
    [SerializeField] float forwardSpeed;
    [SerializeField] float driftSpeed; // eventually might be used to make it travel in a cool spiral instead of a lame baby straight line
    [SerializeField] float verticalFloatSpeed; // when it's full it'll rise slowly
    void Awake()
    {
        bubbleCollider = GetComponent<SphereCollider>();
        bubbleCollider.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isInMovement){
            Vector3 movementVector = new Vector3(0,0,0);
            if(!isFull){
                // Forward part of the movement
                movementVector = transform.forward * forwardSpeed;

                // Spirally drift movement TODO
                // movementVector += transform.up * driftSpeed * Mathf.Sin(Time.time);
                // movementVector += transform.right * driftSpeed * Mathf.Cos(Time.time);
            }
            else{ // i.e. if(isFull)
                movementVector += Vector3.up * verticalFloatSpeed;
            }

            transform.position += (movementVector * Time.deltaTime);
        }
    }

    public void OnGunRelease()
    {
        isInMovement = true;
        bubbleCollider.enabled = true;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, movementDirection*5);
    }
}
