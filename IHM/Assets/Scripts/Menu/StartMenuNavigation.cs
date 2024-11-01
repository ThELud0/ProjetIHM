using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuNavigation : MonoBehaviour
{
    public Button startButton;  // The button to be selected by default
    public Button quitButton;
    public Button settingsButton;
    public GameObject settingsPanel;
    public Button settingsReturnButton;

    private GameObject previousSelectedButton;
    private Button selectedButton;
    private bool isSettingsOpen; // Flag to track if settings are open

    private void Start()
    {
        FeedbackAnimationParameters.health = 0;
        // Automatically select the Start button when the menu loads
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        startButton.onClick.AddListener(LoadLevel);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(LoadSettings);
        settingsReturnButton.onClick.AddListener(CloseSettings);

        settingsPanel.SetActive(false);
        isSettingsOpen = false;
    }



    // Method to load the Level 1 scene
    public void LoadLevel()
    {
        SceneManager.LoadScene("level_1");
    }

    // Method to load the Settings panel
    public void LoadSettings()
    {
        if (!isSettingsOpen)
        {
 

            // Store the currently selected button in the main menu
            previousSelectedButton = EventSystem.current.currentSelectedGameObject;

            // Show the pop-up panel
            settingsPanel.SetActive(true);

            SetStartMenuButtonsState(false);
   

            // Set the flag to indicate settings are open
            isSettingsOpen = true;

            // Select the first button on the pop-up panel
            EventSystem.current.SetSelectedGameObject(settingsReturnButton.gameObject);
        }
    }

    // Method to quit the application
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will log in the console when quitting
        Application.Quit(); // Quit the application
    }

    public void CloseSettings()
    {
        if (isSettingsOpen)
        {


            // Hide the settings panel
            settingsPanel.SetActive(false);

            SetStartMenuButtonsState(true);

            // Restore the previously selected button in the main menu

            EventSystem.current.SetSelectedGameObject(previousSelectedButton);

            isSettingsOpen = false;

        }
    }

    private void SetStartMenuButtonsState(bool state)
    {
        startButton.gameObject.SetActive(state);
        settingsButton.gameObject.SetActive(state);
        quitButton.gameObject.SetActive(state);
    }

}