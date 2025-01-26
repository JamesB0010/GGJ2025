using System;
using EvolveGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChecngeRadioStation : MonoBehaviour
{
    [SerializeField] GameObject Robot;
    [SerializeField] Camera Camera;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == Robot)
                {
                  ChangeRadioStation?.Invoke();
                  this.animator.SetTrigger("PressSkip");
                }
            }
        }
    }

    [SerializeField] public UnityEvent ChangeRadioStation;
}
