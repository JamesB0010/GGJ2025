using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class LerpPackage
{
    
    public Action<LerpPackage> finalCallback;


    public event Action<LerpPackage> OnLerpComplete;
    
    public float timeToLerp;
    public float elapsedtime;
    public float currentPercentage;
    public AnimationCurve animCurve;
    
    public abstract object start { get; set; }
    public abstract object target { get; set; }

    public void Reverse()
    {
        (this.start, this.target) = (this.target, this.start);
        this.ResetTiming();
    }

    public void ResetTiming()
    {
        this.currentPercentage = 0.0f;
        this.elapsedtime = 0.0f;
    }

    protected LerpPackage(Action<LerpPackage> finalCb, float timeToLerp, AnimationCurve animCurve)
    {
        this.finalCallback = finalCb;
        this.timeToLerp = timeToLerp;
        this.animCurve = animCurve;
        this.animCurve ??= GlobalLerpProcessor.linearCurve;
    }

    public void Finished()
    {
        this.finalCallback(this);
        this.OnLerpComplete?.Invoke(this);
    }

    public abstract void AddToProcessor(ref LerpPackageProcessor processor);

    public abstract void RunStepCallback();
}