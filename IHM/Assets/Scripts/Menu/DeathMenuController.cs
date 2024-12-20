using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour
{
    public Button restartButton;  // The button to be selected by default
    public Button quitButton;
    public Button menuButton;



    private void Start()
    {
        FeedbackAnimationParameters.health = 0;
        // Automatically select the Start button when the menu loads
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);

        restartButton.onClick.AddListener(LoadLevel);
        quitButton.onClick.AddListener(QuitGame);
        menuButton.onClick.AddListener(ReturnToStartMenu);

    }



    // Method to load the Level 1 scene
    public void LoadLevel()
    {
        FeedbackAnimationParameters.sceneTransition = true;
        Invoke("LoadLevel1", 1f);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Lvl1");
    }

    public void ReturnToStartMenu()
    {
        FeedbackAnimationParameters.sceneTransition = true;
        Invoke("LoadStartMenu", 1f);
        
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }


    // Method to quit the application
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will log in the console when quitting
        Application.Quit(); // Quit the application
    }


}
