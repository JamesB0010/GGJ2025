using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEndgame : MonoBehaviour
{
    [SerializeField] GameObject endgamePrompt;
    [SerializeField] QuestManager questManager;
    private bool withinRange = false;

    void Start()
    {
        endgamePrompt.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(withinRange && questManager.questComplete)
        {
            // show a prompt on UI to Press E
            endgamePrompt.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E)){
                // Actually end the game
                Cursor.visible = true;
                Cursor.lockState  = CursorLockMode.None;
                SceneManager.LoadScene(2); // make sure build settings allow for this to work.
            }
        }
        else{
            // hide the UI prompt
            endgamePrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider _c)
    {
        if(_c.transform.tag == "Drop Pod")
        {
            withinRange = true;
            Debug.Log("Player is in range of drop pod");
        }
    }
    private void OnTriggerExit(Collider _c)
    {
        if(_c.transform.tag == "Drop Pod")
        {
            withinRange = false;
            Debug.Log("Player isnt in range of drop pod");
        }
    }
}
