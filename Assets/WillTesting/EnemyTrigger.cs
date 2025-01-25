using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private GameObject questManager;
    private QuestManager questScript;

    private void Start()
    {
        questManager = GameObject.Find("Quest Manager");
        questScript = questManager.GetComponent<QuestManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GameController"))
        {
            questScript.CreatureCollected();
            Destroy(this.gameObject);
        }
    }
}