using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object Value References/Boolean")]
public class BoolReference : SuperBaseScriptableValRef<bool>
{
    private bool value;

    public override bool GetValue()
    {
        return value;
    }

    public void SetValue(bool val)
    {
        this.value = val;
    }
}
