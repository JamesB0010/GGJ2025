using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LilGuyCaptured : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Vector3 rotationDirection;

    public void Enter(State state)
    {
        this.rotationDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    public void Behave(State state)
    {
        transform.Rotate(this.rotationDirection, this.rotationSpeed * Time.deltaTime);
    }
}
