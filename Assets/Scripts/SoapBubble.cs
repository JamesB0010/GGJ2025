using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoapBubble : MonoBehaviour
{
    [SerializeField] bool isInMovement = false; // turned on after being fired
    [SerializeField] public bool isFull = false; // turned on after colliding with a damp creature
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
    void OnTriggerEnter(Collider _c)
    {
        Debug.Log("Trigger Enter Called");

        if(_c.tag == "Capturable"){
            Debug.Log($"Collision with Big Bubble against {_c.gameObject.name}");
            CatchableAgent contactAgent  = _c.gameObject.GetComponent<CatchableAgent>();
            if(contactAgent == null){Debug.Log($"Unable to extract CatchableAgent from {_c.gameObject.name}");}
            // 1: bubble is big enough |-| 2: Target is moist enough |-| 3: Bubble is not already full
            if(!isFull && contactAgent.moistness >= contactAgent.moistnessThreshold && transform.localScale.x >= contactAgent.minBubbleSize){
                // Capture the thing:
                // Snap bubble to creature center
                transform.position = contactAgent.transform.position;
                // probably do some size snapping, either by shrinking this or expanding the bubble
                // child this object to the bubble
                contactAgent.transform.parent = transform;
                // make the bubble float upwards
                isFull = true;
                contactAgent.isCaptured = true;
            }
            // else pop the contactBubble
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
