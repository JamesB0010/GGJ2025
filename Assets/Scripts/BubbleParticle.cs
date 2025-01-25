using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] List<ParticleCollisionEvent> collEvents;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collEvents = new List<ParticleCollisionEvent>();
    }
    private void OnParticleCollision(GameObject _hitObj)
    {
        Debug.Log($"{_hitObj.name} was splashed");
        // TODO When animals are implemented, make them damp via this function.
        // Also make them visually slippy
        Material hitMat = _hitObj.GetComponent<Renderer>().material;
        if(hitMat.GetFloat("_Moistness") < 1){
            hitMat.SetFloat("_Moistness", hitMat.GetFloat("_Moistness")+0.1f);
        }
    }
}
