using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Radio : MonoBehaviour
{
    [SerializeField] List<AudioSource> RadioStations;
    [SerializeField] AudioSource RadioChangeSound;
    private int itterator = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var Station in RadioStations)
        {
            Station.volume = 0;
            Station.Play();
        }
        RadioStations[itterator].volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangeRadioStation(GameObject InTarget)
    {
        RadioStations[itterator].volume = 0;
        itterator++;
        RadioChangeSound.Play();
    }
}
