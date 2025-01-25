using System;
using System.Collections;
using System.Collections.Generic;
using EvolveGames;
using UnityEngine;

public class LilGuySharedData : MonoBehaviour
{
    private Transform player;

    public Transform Player => this.player;

    private void Start()
    {
        this.player = FindObjectOfType<PlayerController>().transform;
    }
}
