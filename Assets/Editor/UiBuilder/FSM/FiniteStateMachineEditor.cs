using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineEditor : EditorWindow
{
    private FSMGraphView graphView;
    private FSMInspectorView inspectorView;
    
    private FSMStateView lastSelected = null;
    
    
    [MenuItem("Custom Ai Behaviour/FSM Graph Editor")]
    public static void OpenWindow()
    {
        FiniteStateMachineEditor wnd = GetWindow<FiniteStateMachineEditor>();
        wnd.titleContent = new GUIContent("FSM Graph Editor");
    }
    
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is FiniteStateMachine)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        //Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UiBuilder/FSM/FSMGraphEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UiBuilder/FSM/FSMGraphEditorStyles.uss");
        root.styleSheets.Add(styleSheet);


        this.graphView = root.Q<FSMGraphView>();
        this.graphView.StateViewsCreated += this.OnStateViewsCreated;


        this.graphView.unselectAll += () => this.inspectorView.Clear();

        this.inspectorView = root.Q<FSMInspectorView>();

        this.graphView.OnNodeSelected = OnNodeSelectionChanged;
        
        this.OnSelectionChange();
    }

    private void OnStateViewsCreated()
    {
        this.graphView.edgeSelected += this.EdgeSelected;
    }

    private void EdgeSelected(FSMConnectionView connection)
    {
        this.inspectorView.EdgeSelected(connection);
        connection.output.node.AddToClassList("StateFromNode");
    }


    //when the user changes selection in unitys assets browser
    private void OnSelectionChange()
    {
        FiniteStateMachine fsm = Selection.activeObject as FiniteStateMachine;

        if (!fsm)
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.TryGetComponent(out FSMMonoComponent monoComp))
                    fsm = monoComp.FiniteStateMachine;
            }

        if (Application.isPlaying)
        {
            if (fsm != null && this.graphView != null)
                this.graphView.PopulateView(fsm);
        }
        else if (fsm && AssetDatabase.CanOpenAssetInEditor(fsm.GetInstanceID()))
        {
            if(fsm == null || this.graphView == null)
                return;
            this.graphView.PopulateView(fsm);
        }
    }

    private void OnNodeSelectionChanged(FSMStateView stateView)
    {
        this.inspectorView.UpdateSelection(stateView);
        if (stateView.State is State s)
        {
            s.stateConnections.ForEach(con =>
            {
                StateCustomEditor stateCustomEditor = ((StateCustomEditor)this.inspectorView.Editor);

                if (con.AgentPrefab != null)
                    stateCustomEditor.GetEvaluatorsOnAgentPrefab(con.AgentPrefab);
            });
        }
        this.lastSelected?.RemoveFromClassList("Selected");
        stateView.AddToClassList("Selected");
        this.lastSelected = stateView;
        stateView.MarkDirtyRepaint();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                this.OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                this.OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                this.SelectPrototypeOfThisFsm();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }

    private void SelectPrototypeOfThisFsm()
    {
        //not looking at a state machine anyway so no need to repopulate using prototype
        if (this.graphView == null)
            return;
        
        //this repopulates the view using the fsm which was used to create the current clone
        this.graphView.PopulateView(this.graphView.Fsm.Prototype, true);
    }

    private void OnInspectorUpdate()
    {
        if (Application.isPlaying)
            return;
        
        if(this.lastSelected == null)
            return;
        
        var lastSelectedNameGui = this.lastSelected.Q<Label>("title-label");
        if (lastSelectedNameGui.text != this.lastSelected.State.name)
            lastSelectedNameGui.text = this.lastSelected.State.name;
    }
}
