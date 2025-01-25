using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionLerpPackage : LerpPackage
{
    public event Action<Quaternion> onLerpStep;
    
    public Action<Quaternion> lerpStepCallback;

    private Quaternion _start, _target;

    public override object start
    {
        get => this._start;
        set => this._start = (Quaternion)value;
    }

    public override object target
    {
        get => this._target;
        set => this._target = (Quaternion)value;
    }

    public QuaternionLerpPackage(Quaternion start, Quaternion target, Action<Quaternion> stepCallback, Action<LerpPackage> finalCb,
        float timeToLerp = 1.0f, AnimationCurve animCurve = null) : base(finalCb, timeToLerp, animCurve)
    {
        this._start = start;
        this._target = target;
        this.lerpStepCallback = stepCallback;
    }

    public override void AddToProcessor(ref LerpPackageProcessor processor)
    {
        processor.AddPackage(this);
    }

    public override void RunStepCallback()
    {
        Quaternion value = Quaternion.Lerp(this._start, this._target, this.currentPercentage);
        this.onLerpStep?.Invoke(value);
        this.lerpStepCallback(value);
    }
}