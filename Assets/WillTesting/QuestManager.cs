using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestManager : MonoBehaviour
{
    [Header("Creatures")]
    public GameObject[] featuredCreatures;
    public int numberOfCreatures = 3;
    private GameObject[] creatures;

    [Header("Questing")]
    public int requiredCreatures;
    public int currentCreatures = 0;
    private bool questComplete;


    // Start is called before the first frame update
    void Start()
    {
        // Setting Creatures needed to be collected to number of spawned ones
        requiredCreatures = numberOfCreatures;

        // Used to spawn in creatures
        CreatureSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        // Running the Quest
        CurrentQuest();
    }

    private void CurrentQuest()
    {
        // Checking if player collected all creatures
        if (currentCreatures >= requiredCreatures)
        {
            questComplete = true;
        }
    }

    public void CreatureCollected()
    {
        currentCreatures++;
    }

    private void CreatureSpawn()
    {
        creatures = new GameObject[numberOfCreatures];

        for (int i = 0; i < numberOfCreatures; i++)
        {
            // Go through creatures that should be in the level and randomly adding them
            int creatureToAdd = Random.Range(0, featuredCreatures.Length);

            // Add creature to scene
            GameObject newCreature = Instantiate(featuredCreatures[creatureToAdd], transform.position, Quaternion.identity);

            // Add creature to creatures array
            creatures[i] = newCreature;
        }
    }
}
