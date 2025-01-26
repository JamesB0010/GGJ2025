using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    [Header("Creatures")]
    public List<ChecklistEntity> creaturePrefabs = new List<ChecklistEntity>();
    public int spawnCount = 3;
    private GameObject[] activeCreatures;
    private BoxCollider spawnArea;
    private Dictionary<GameObject, int> creatureDictionary;

    [Header("Questing")]
    public int requiredCreatures;
    public int currentCreatures = 0;
    private bool questComplete;

    [Header("UI")]
    public Transform paperContent;
    public GameObject creatureEntryPrefab;
    [SerializeField] GameObject[] uiEntries;

    // Start is called before the first frame update
    void Start()
    {
        // Setting Creatures needed to be collected to number of spawned ones
        requiredCreatures = spawnCount;
        uiEntries = new GameObject[creaturePrefabs.Count];

        // Get the box collider on gameobject
        spawnArea = GetComponent<BoxCollider>();

        // Init Dic
        creatureDictionary = new Dictionary<GameObject, int>();

        // Used to spawn in creatures
        CreatureSpawn();

        // Create the list with initial creatures
        InitialiseCreatureListUI();
    }

    // Update is called once per frame
    void Update()
    {   
        //foreach (var key in creatureDictionary.Keys)
        //{
        //    foreach (var value in creatureDictionary.Values)
        //    {
        //        Debug.Log("Creature: " + key + "Number In Scene: " + value);
        //    }
        //}
    }

    private void CurrentQuest()
    {
        // Checking if player collected all creatures
        if (currentCreatures >= requiredCreatures)
        {
            questComplete = true;
        }
    }

    // Call when creatures are collected
    public void CreatureCollected()
    {
        currentCreatures++;
        CurrentQuest();
    }

    private void CreatureSpawn()
    {
        activeCreatures = new GameObject[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            // Go through creatures that should be in the level and randomly adding them
            int prefabIndex = Random.Range(0, creaturePrefabs.Count);
            ChecklistEntity currentCreature = creaturePrefabs[prefabIndex];
            GameObject creaturePrefab = currentCreature.gameObject;

            // Spawn creatures in area of box collider, this size is set in the inspector
            var spawnBounds = spawnArea.bounds;
            var areaX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            var areaZ = Random.Range(spawnBounds.min.z, spawnBounds.max.z);
            Vector3 spawnPos = new Vector3(areaX, transform.position.y, areaZ);

            // Add creature to scene
            GameObject newCreature = Instantiate(creaturePrefab, spawnPos, Quaternion.identity);

            // Add creature to dictionary for tracking
            if (creatureDictionary.ContainsKey(creaturePrefab))
            {
                creatureDictionary[creaturePrefab]++;
            }
            else
            {
                creatureDictionary.TryAdd(creaturePrefab, 1);
            }
        }
    }

    // Get the creature count, used for tracking number of certain creatures in the scene
    private int GetCreatureCount(GameObject creaturePrefabCheck)
    {
        return creatureDictionary[creaturePrefabCheck];
    }

    private void DeductCreatureCount(GameObject creaturePrefabCheck)
    {
        creatureDictionary[creaturePrefabCheck]--;
    }

    private void InitialiseCreatureListUI()
    {
        for (int i = 0; i < creaturePrefabs.Count; i++){
            if (creatureDictionary.ContainsKey(creaturePrefabs[i].gameObject))
            {
                uiEntries[i] = Instantiate(creatureEntryPrefab, paperContent);

                TMP_Text creatureNameText = uiEntries[i].transform.Find("Name").GetComponent<TMP_Text>();
                creatureNameText.text = creaturePrefabs[i].data.name;

                Image creatureImageBox = uiEntries[i].transform.Find("CreatureImg").GetComponent<Image>();
                creatureImageBox.sprite = creaturePrefabs[i].data.image;

                TMP_Text creatureCounter = uiEntries[i].transform.Find("CaptureCount").GetComponent<TMP_Text>();
                creatureCounter.text = GetCreatureCount(creaturePrefabs[i].gameObject).ToString();
            }
        }
    }

    public void UISubtractAtID(int _id)
    {
        // UI Update
        for(int i = 0; i < uiEntries.Length; i++)
        {
            if (i != _id)
                continue;

            DeductCreatureCount(creaturePrefabs[i].gameObject);

            TMP_Text creatureCounter = uiEntries[i].transform.Find("CaptureCount").GetComponent<TMP_Text>();
            creatureCounter.text = GetCreatureCount(creaturePrefabs[i].gameObject).ToString();
        }

        // Progression Update
        CreatureCollected();
    }
}
