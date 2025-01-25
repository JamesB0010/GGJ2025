using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using PlasticGui.WorkspaceWindow.QueryViews.Branches;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateView : UnityEditor.Experimental.GraphView.Node
{
    public Action<FSMStateView> OnNodeSelected { get; set; }
    
    private Port input;
    private Port output;

    public Port Input => this.input;
    public Port Output => this.output;

    private FSMStateBase state;
    public FSMStateBase State => this.state;

    public event Action<FSMStateView> enteredState;
    public event Action<FSMStateView> exitedState;

    
    public FSMStateView(FSMStateBase state) : base("Assets/Editor/UiBuilder/FSM/FSMNodeView.uxml")
    {
        this.state = state;
        base.title = this.state.name;
        base.viewDataKey = this.state.guid;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UiBuilder/FSM/FSMNodeViewStyle.uss");
        base.styleSheets.Add(styleSheet);
        base.style.left = this.state.graphEditorPosition.x;
        base.style.top = this.state.graphEditorPosition.y;

        if (this.state is State s)
        {
            //need to add these to the list of callbacks
            s.AddToEnterStateCallbackList(this.EnteredState);
            s.AddToExitStateCallbackList(this.ExitedState);
        }
        else
            AddToClassList("Entry");
        
        this.CreateInputPorts();
        this.CreateOutputPorts();

        Label titleLabel = this.Q<Label>("title-label");
        titleLabel.bindingPath = "name";
        titleLabel.Bind(new SerializedObject(this.state));

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(this.state));
    }

    private void EnteredState(State state)
    {
        if (Application.isPlaying)
            this.enteredState?.Invoke(this);
    }

    private void ExitedState(State state)
    {
        if(Application.isPlaying)
            this.exitedState?.Invoke(this);
    }


    private void CreateInputPorts()
    {
        var thisState = this.state as EntryState;
        if (thisState)
        {
            return;
        }
        this.input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));

        if (this.input != null)
        {
            this.input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            base.inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        var thisState = this.state as EntryState;
        if (thisState)
        {
            this.output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else
        {
            this.output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
       
        if (output != null)
        {
            this.output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            base.outputContainer.Add(output);
        }
    }


    public override void OnSelected()
    {
        base.OnSelected();
        this.OnNodeSelected?.Invoke(this);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        this.state.graphEditorPosition.x = newPos.xMin;
        this.state.graphEditorPosition.y = newPos.yMin;
    }
}
