using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosionspeedmodifier : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    void Start()
    {
        ParticleSystem[] childParticles = this.GetComponentsInChildren<ParticleSystem>();
        particles = new ParticleSystem[childParticles.Length];

        for (int i = 0; i < childParticles.Length; i++)
        {
            this.particles[i] = childParticles[i];
        }

        particles[particles.Length - 1] = GetComponent<ParticleSystem>();
    }
}
