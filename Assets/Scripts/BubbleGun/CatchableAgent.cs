using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatchableAgent : MonoBehaviour
{
    [SerializeField] public float moistness;
    [SerializeField] public float moistnessThreshold; // how moist does it have to be to capture
    [SerializeField] public float minBubbleSize; // how big does the bubble have to be 
    [SerializeField] float dryingRate; // how much does moistness decrease per second
    [SerializeField] public bool isCaptured = false;
    public delegate void OnGetCaptured();
    public OnGetCaptured captured;
    [SerializeField] public Renderer agentRenderer;
    [SerializeField] public int slippyMatIndex;
    [SerializeField] public Transform bubbleAnchor;


    void Start()
    {
        captured = GetCaptured;
        SetSlipMatIndex();
    }

    void Update()
    {
        if(moistness > 0 && !isCaptured){
            moistness -= dryingRate * Time.deltaTime;
            // reduce moistness of material as well
            Material slippyMat = agentRenderer.materials[slippyMatIndex];
            if(slippyMat.GetFloat("_Moistness") > 0){
                slippyMat.SetFloat("_Moistness", slippyMat.GetFloat("_Moistness")-(dryingRate*Time.deltaTime));
            }
        }
    }

    void GetCaptured()
    {
        FSMMonoComponent runner = GetComponent<FSMMonoComponent>();
        runner.TransitionActiveState(1);
    }

    private void SetSlipMatIndex()
    {
        for(int i = 0; i < agentRenderer.materials.Length; i++){
            if(agentRenderer.materials[i].shader == Shader.Find("Shader Graphs/SplashableSG")){
                slippyMatIndex = i;
                return;
            }
        }
    }
}
