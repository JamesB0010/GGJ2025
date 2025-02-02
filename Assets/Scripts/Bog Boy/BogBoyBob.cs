using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BogBoyBob : MonoBehaviour
{
    [SerializeField] private float bobMagnitude;

    [SerializeField] private float bobSpeed;
    [SerializeField] CatchableAgent captureStatus;
    void Update()
    {
        if(!captureStatus.isCaptured){
            Vector3 currentPos = transform.position;

            float yPosAdditive = Mathf.Sin(Time.timeSinceLevelLoad * this.bobSpeed);

            transform.position = currentPos + Vector3.up * (yPosAdditive * bobMagnitude);
        }
        
    }
}
