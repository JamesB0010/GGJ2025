using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Bloom = UnityEngine.Rendering.Universal.Bloom;

public class BloomOnExplosion : MonoBehaviour
{
    [SerializeField] private Volume volume;

    [SerializeField] private AnimationCurve explosionBloomFalloff;

    [SerializeField] private float explosionBloomFalloffTime;

    public void OnExplosion()
    {
        Bloom bloom = volume.profile.components[0] as Bloom;
        bloom.intensity.value = 62;

        bloom.intensity.value.LerpTo(0.0f, explosionBloomFalloffTime, val => bloom.intensity.value = val, null,
            this.explosionBloomFalloff);
    }
}
