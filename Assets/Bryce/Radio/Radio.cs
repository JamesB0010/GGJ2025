using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Radio : MonoBehaviour
{
    [SerializeField] List<AudioSource> RadioStations;
    [SerializeField] AudioSource RadioChangeSound;
    private int itterator = 0;

    [SerializeField] private bool radioEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var Station in RadioStations)
        {
            Station.volume = 0;
        }
        RadioStations[itterator].volume = 1;

        if (!radioEnabled)
            RadioStations[itterator].volume = 0;
    }

    public void ChangeRadioStation()
    {
        Debug.Log("Change Radio Stateion");
        RadioStations[itterator].volume = 0;
        itterator = (itterator + 1) % RadioStations.Count;
        RadioStations[itterator].volume = 1;
        RadioChangeSound.Play();
    }
}
