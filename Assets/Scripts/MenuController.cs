using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadMainGame()
    {
        SceneManager.LoadScene(0); // check build settings.
    }
    
    public void Quit()
    {
        Debug.Log("See ya");
        Application.Quit();
    }
}
