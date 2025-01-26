using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosionspeedmodifier : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;

    [SerializeField] private float startSpeed, endSpeed;

    [SerializeField] private float timeToTake;

    [SerializeField] private AnimationCurve slowdownRate;

    private ParticleSystem rootParticleSystem;
    void Start()
    {
        for (int i = 0; i < this.particles.Length; i++)
        {
            var main = particles[i].main;
            main.simulationSpeed = this.startSpeed;
            main.simulationSpeed.LerpTo(this.endSpeed, this.timeToTake, val => main.simulationSpeed = val, null,
                this.slowdownRate);
        }

        this.rootParticleSystem = GetComponent<ParticleSystem>();

        var audio = GetComponent<AudioSource>();
        audio.volume.LerpTo(0.0f, 2f, val => audio.volume = val);
    }

    private void Update()
    {
        if (this.rootParticleSystem.time >= 0.94f)
        {
            Destroy(this.gameObject);
        }
    }
}
