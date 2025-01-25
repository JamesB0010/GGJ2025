using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class FSMGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<FSMGraphView, GraphView.UxmlTraits>
    {
    }

    private FiniteStateMachine fsm;
    public FiniteStateMachine Fsm => this.fsm;
    public Action<FSMStateView> OnNodeSelected { get; set; }

    public event Action unselectAll;
    
    public event Action StateViewsCreated;

    public event Action<FSMConnectionView> edgeSelected;
    public FSMGraphView()
    {
        base.Insert(0, new GridBackground());
        
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        base.RegisterCallback<PointerDownEvent>(this.OnPointerDown);
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UiBuilder/FSM/FSMGraphEditorStyles.uss");
        base.styleSheets.Add(styleSheet);

    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //background clicked
        if(evt.button == 0)
            this.DeselectAllNodes();
    }

    private void DeselectAllNodes()
    {
        //no fsm in editor, therefore no nodes to select, therefore no need to deselect any nodes
        if (this.fsm == null)
            return;
        
        this.fsm.ActiveState = null;
        this.fsm.States.ForEach(state =>
        {
            var node = GetNodeByGuid(state.guid);
            node.RemoveFromClassList("Selected");
            node.RemoveFromClassList("StateFromNode");
        });
        this.unselectAll?.Invoke();
    }

    private FSMStateView FindStateView(FSMStateBase state)
    {
        return base.GetNodeByGuid(state.guid) as FSMStateView;
    }


    public void PopulateView(FiniteStateMachine fsm, bool assumeIsPlayingFalse = false)
    {
        this.fsm = fsm;
        if (fsm == null)
            return;
        
        graphViewChanged -= OnGraphViewChanged;
        base.DeleteElements(base.graphElements);
        graphViewChanged += OnGraphViewChanged;

        
        if (this.fsm.EntryState == null)
        {
            this.fsm.CreateEntryState();
            EditorUtility.SetDirty(this.fsm);
            AssetDatabase.SaveAssets();
        }

        base.focusable = true;
        
        //creates node views
        this.fsm.States.ForEach(this.CreateStateView);
        
        this.StateViewsCreated?.Invoke();
        //creates edges
        CreateVisualStateConnections();
        

        if (assumeIsPlayingFalse)
            return;
        
        if (Application.isPlaying && this.fsm.ActiveState != null)
            GetNodeByGuid(this.fsm.ActiveState.guid).AddToClassList("Running");
    }
    
    private void CreateVisualStateConnections()
    {
        this.fsm.States.ForEach(state =>
        {
            FSMStateView fromView = this.FindStateView(state);
            switch (state)
            {
                case EntryState entryState:
                    if (entryState.stateConnection.StateTo == null)
                        return;
                    FSMStateView toView = this.FindStateView(entryState.stateConnection.StateTo);
                    
                    if (toView.Input == null)
                        return;

                    FSMConnectionView edge = this.CreateFsmConnection(fromView.Output, toView.Input);
                    base.AddElement(edge);
                    break;
                case State normalState:
                    normalState.stateConnections.ForEach(connection =>
                    {
                        FSMStateView toView = this.FindStateView(connection.StateTo);
                        if (toView.Input == null)
                            return;

                        FSMConnectionView edge = this.CreateFsmConnection(fromView.Output, toView.Input);
                        base.AddElement(edge);
                    });
                    break;
            }
        });
    }


    private FSMConnectionView CreateFsmConnection (Port output, Port input)
    {
        FSMConnectionView edge = new FSMConnectionView()
        {
            output = output,
            input = input
        };

        edge.OnEdgeSelected += this.edgeSelected;
        output.Connect(edge);
        input.Connect(edge);
        
        return edge;
    }

    private void CreateStateView(FSMStateBase state)
    {
        FSMStateView stateView = new FSMStateView(state);
        stateView.OnNodeSelected = this.OnNodeSelected;
        stateView.enteredState += this.StateEntered;
        stateView.exitedState += this.StateExit;
        base.AddElement(stateView);
    }

    private void StateEntered(FSMStateView enteredState)
    {
        enteredState.AddToClassList("Running");
    }

    private void StateExit(FSMStateView exitState)
    {
        exitState.RemoveFromClassList("Running");
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return base.ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        DeleteThingsToBeDeleted(graphViewChange);

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                FSMStateView start = edge.output.node as FSMStateView;
                FSMStateView to = edge.input.node as FSMStateView;
                
                this.fsm.MakeConnection(start.State, to.State);
            });
        }
        return graphViewChange;
    }

    private void DeleteThingsToBeDeleted(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            //if true is returned inside this callback function it means the element that was going to be deleted is removed from the list of elements to delete
            //and is therefore not deleted. anything that does not return true (returns false) will be deleted
            graphViewChange.elementsToRemove.RemoveAll(elem =>
            {
                FSMStateView stateView = elem as FSMStateView;
                if (stateView != null)
                {
                    State state = stateView.State as State;
                    if (state == null)
                    {
                        return true;
                    }
                    else
                    {
                        this.fsm.DeleteState(state);
                    }
                }

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    FSMStateView start = edge.output.node as FSMStateView;
                    FSMStateView to = edge.input.node as FSMStateView;
                    
                    this.fsm.BreakConnection(start.State, to.State);
                }

                return false;
            });
            
            
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        evt.menu.AppendAction("Add New State", action => this.CreateNode());
    }

    private void CreateNode()
    {
        State state = fsm.CreateState();
        this.CreateStateView(state);

        //it is 2 because there will always be at least 1 state (entry state)
        bool firstStateCreated = this.fsm.States.Count == 2;
        if (firstStateCreated)
        {
            this.fsm.MakeConnection(this.fsm.EntryState, state);
            this.CreateVisualStateConnections();
        }
    }
}
