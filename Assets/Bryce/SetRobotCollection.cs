using EvolveGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetRobotCollection : MonoBehaviour
{
    [SerializeField] ItemChange ItemSelect;
    [SerializeField] LayerMask BubbleLayers;
    [SerializeField] Camera Camera;
    [SerializeField] BoolReference isCollecting;
    private Animator animator;

    private void Start()
    {
        ItemSelect = GetComponentInParent<PlayerController>().GetComponent<ItemChange>();
        this.animator = GetComponent<Animator>();
    }

    void Update()
    {
        Ray ray = Camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if(ItemSelect.ItemIdInt == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, BubbleLayers))
                {
                    if (hit.collider.TryGetComponent<SoapBubble>(out SoapBubble Bubble))
                    {
                        CatchableAgent agent = Bubble.GetComponentInChildren<CatchableAgent>();
                        if (agent != null)
                        {
                            GetTarget?.Invoke(hit.collider.gameObject);
                            isCollecting.SetValue(true);
                            this.animator.SetTrigger("PressRed");
                        }
                    }
                }
            }
        }
    }

    [SerializeField] public UnityEvent<GameObject> GetTarget;
}
