using System;
using System.Collections;
using System.Collections.Generic;
using EvolveGames;
using UnityEngine;

public class LilGuySharedData : MonoBehaviour
{
    [SerializeField] private Transform player;

    public Transform Player => this.player;

    private void Awake()
    {
        this.player = FindObjectOfType<PlayerController>().transform;
    }
}
