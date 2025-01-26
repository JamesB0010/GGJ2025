using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChecklistEntity : MonoBehaviour
{
    [System.Serializable] public struct CreatureData
    {
        public int id;
        public string name;
        public Sprite image;
    }

    [SerializeField] public CreatureData data;
    [SerializeField] private GameObject questManager;
    private QuestManager questScript;

    private void Start()
    {
        questManager = GameObject.Find("Quest Manager");
        questScript = questManager.GetComponent<QuestManager>();
    }

    public void OnCollect()
    {
        Debug.Log($"Creature Collected: {name}");
        questScript.UISubtractAtID(data.id);
    }
}