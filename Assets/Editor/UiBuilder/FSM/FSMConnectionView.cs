using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSMConnectionView : UnityEditor.Experimental.GraphView.Edge
{
    public event Action<FSMConnectionView> OnEdgeSelected;

    public override void OnSelected()
    {
        base.OnSelected();
        this.OnEdgeSelected?.Invoke(this);
    }
}
