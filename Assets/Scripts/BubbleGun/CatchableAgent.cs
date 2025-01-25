using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchableAgent : MonoBehaviour
{
    [SerializeField] public float moistness;
    [SerializeField] public float moistnessThreshold; // how moist does it have to be to capture
    [SerializeField] public float minBubbleSize; // how big does the bubble have to be 
    [SerializeField] float dryingRate; // how much does moistness decrease per second
    [SerializeField] public bool isCaptured = false;

    void Update()
    {
        if(moistness > 0 && !isCaptured){moistness -= dryingRate * Time.deltaTime;}
    }
}
