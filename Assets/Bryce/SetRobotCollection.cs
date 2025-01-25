using EvolveGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetRobotCollection : MonoBehaviour
{
    ItemChange ItemSelect;
    [SerializeField] Camera Camera;
    private void Start()
    {
        ItemSelect = GetComponentInParent<PlayerController>().GetComponent<ItemChange>();
    }

    void Update()
    {
        Ray ray = Camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (ItemSelect.ItemIdInt == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.TryGetComponent<SoapBubble>(out SoapBubble Bubble))
                    {
                        CatchableAgent agent = Bubble.GetComponentInChildren<CatchableAgent>();
                        if (agent != null)
                        {
                            GetTarget?.Invoke(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    [SerializeField] public UnityEvent<GameObject> GetTarget;
}
