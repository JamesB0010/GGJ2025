using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatchableAgent : MonoBehaviour
{
    [SerializeField] public float moistness;
    [SerializeField] public float moistnessThreshold; // how moist does it have to be to capture
    [SerializeField] public float minBubbleSize; // how big does the bubble have to be 
    [SerializeField] float dryingRate; // how much does moistness decrease per second
    [SerializeField] public bool isCaptured = false;
    public delegate void OnGetCaptured();
    public OnGetCaptured captured;


    void Start()
    {
        captured = GetCaptured;
    }

    void Update()
    {
        if(moistness > 0 && !isCaptured){moistness -= dryingRate * Time.deltaTime;}
    }

    void GetCaptured()
    {
        Debug.Log($"Event Working. {name} got captured.");
        // @ James do stuff here.
    }
}
