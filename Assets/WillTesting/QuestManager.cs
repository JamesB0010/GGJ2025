using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class Creature
{
    public string name;
    public Sprite creatureImage;
    public GameObject creaturePrefab;
}

public class QuestManager : MonoBehaviour
{
    [Header("Creatures")]
    public List<Creature> creaturesList = new List<Creature>();
    public int numberOfCreatures = 3;
    private GameObject[] creatures;
    private BoxCollider spawnArea;
    private Dictionary<GameObject, int> creatureDictionary;

    [Header("Questing")]
    public int requiredCreatures;
    public int currentCreatures = 0;
    private bool questComplete;

    [Header("UI")]
    public Transform paperContent;
    public GameObject creatureEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Setting Creatures needed to be collected to number of spawned ones
        requiredCreatures = numberOfCreatures;

        // Get the box collider on gameobject
        spawnArea = GetComponent<BoxCollider>();

        // Init Dic
        creatureDictionary = new Dictionary<GameObject, int>();

        // Used to spawn in creatures
        CreatureSpawn();

        // Create the list with initial creatures
        UpdateCreatureListUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Running the Quest
        CurrentQuest();
        
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

        // Only update on collection
        UpdateCreatureListUI();
    }

    private void CreatureSpawn()
    {
        creatures = new GameObject[numberOfCreatures];

        for (int i = 0; i < numberOfCreatures; i++)
        {
            // Go through creatures that should be in the level and randomly adding them
            int creatureToAdd = Random.Range(0, creaturesList.Count);
            Creature currentCreature = creaturesList[creatureToAdd];
            GameObject creaturePrefab = currentCreature.creaturePrefab;

            // Spawn creatures in area of box collider, this size is set in the inspector
            var spawnBounds = spawnArea.bounds;
            var areaX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
            var areaZ = Random.Range(spawnBounds.min.z, spawnBounds.max.z);
            Vector3 spawnPos = new Vector3(areaX, transform.position.y, areaZ);

            // Add creature to scene
            GameObject newCreature = Instantiate(currentCreature.creaturePrefab, spawnPos, Quaternion.identity);

            // Add creature to creatures array
            creatures[i] = newCreature;

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

    private void UpdateCreatureListUI()
    {
        // Remove previous entries
        foreach (Transform child in paperContent)
        {
            Destroy(child.gameObject);
        }

        // Add each creature to the ui list
        foreach (var creature in creaturesList)
        {
            // Only add to list if the creature exists in the dictionary
            if (creatureDictionary.ContainsKey(creature.creaturePrefab))
            {
                // Create entry from prefab and add to display
                GameObject creatureEntry = Instantiate(creatureEntryPrefab, paperContent);

                // Set the image and name of the creature
                TMP_Text creatureNameText = creatureEntry.transform.Find("Name").GetComponent<TMP_Text>();
                Image creatureImageBox = creatureEntry.transform.Find("CreatureImg").GetComponent<Image>();
                TMP_Text creatureCounter = creatureEntry.transform.Find("CaptureCount").GetComponent<TMP_Text>();

                // If the creature name is found then change it
                if (creatureNameText != null)
                {
                    creatureNameText.text = creature.name;
                }
                else
                {
                    Debug.Log("Can't find name");
                }

                // If the creature image is found then change it
                if (creatureImageBox != null)
                {
                    creatureImageBox.sprite = creature.creatureImage;
                }
                else
                {
                    Debug.Log("Can't find image");
                }

                // If the creature count is found then change it
                if (creatureCounter != null)
                {
                    creatureCounter.text = GetCreatureCount(creature.creaturePrefab).ToString();
                }
                else
                {
                    Debug.Log("Can't find count");
                }
            }
        }
    }
}
