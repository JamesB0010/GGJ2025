using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [System.Serializable] public struct CreatureData
    {
        public int id;
        public string name;
        public Sprite image;
    }

    [SerializeField] public CreatureData data;
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
            questScript.UISubtractAtID(data.id-1);
            Destroy(this.gameObject);
        }
    }
}