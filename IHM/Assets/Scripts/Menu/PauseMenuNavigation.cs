using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuNavigation : MonoBehaviour
{
    public Button resumeButton;  // The button to be selected by default
    public Button homeButton;
    public Button quitButton;
    public Button settingsButton;
    public GameObject settingsPanel;
    public GameObject pauseMenuUI;
    public GameObject helpPanel;
    public GameObject helpIndicationPanel;
    public GameObject healthBar;
    public Button settingsReturnButton;

    private GameObject previousSelectedButton;
    private Button selectedButton;
    private bool isSettingsOpen; // Flag to track if settings are open
    private bool helpPanelPreviousState;
    public static bool isPaused ;



    private void Start()
    {
        // Automatically select the Start button when the menu loads
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(LoadSettings);
        settingsReturnButton.onClick.AddListener(CloseSettings);
        homeButton.onClick.AddListener(ReturnToStartMenu);
        settingsPanel.SetActive(true);
        Invoke("SetSettingsStateFalse", 0.005f); //open settings real quick to get the volume values from sliders to mixer
        pauseMenuUI.SetActive(false);
        isSettingsOpen = false;
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {

        // Check if the player presses the Pause key (Escape or Start on a controller)
        if (Input.GetKeyDown(KeyCode.Escape) || ((Gamepad.current != null) && Gamepad.current.startButton.wasPressedThisFrame))
        {
            if (isPaused && !isSettingsOpen)
            {
                Resume();
            }
            else if (!isSettingsOpen)
            {
                Pause();
            }
        }

    }

    // Resume the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);  // Hide pause menu UI
        Time.timeScale = 1f;  // Resume game time
        isPaused = false;
    }

    // Pause the game
    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show pause menu UI
        Time.timeScale = 0f;  // Freeze game time
        isPaused = true;
    }

    // Method to load the Level 1 scene
    public void ReturnToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    // Method to load the Settings panel
    public void LoadSettings()
    {
        if (!isSettingsOpen)
        {

            helpIndicationPanel.SetActive(false);
            helpPanelPreviousState = helpPanel.activeSelf;
            helpPanel.SetActive(false);
            healthBar.SetActive(false);
            // Store the currently selected button in the main menu
            previousSelectedButton = EventSystem.current.currentSelectedGameObject;

            // Show the pop-up panel
            settingsPanel.SetActive(true);

            SetPauseMenuUIState(false);

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

            SetPauseMenuUIState(true);
            helpIndicationPanel.SetActive(true);
            healthBar.SetActive(true);
            helpPanel.SetActive(helpPanelPreviousState);
            EventSystem.current.SetSelectedGameObject(previousSelectedButton);

            isSettingsOpen = false;


        }
    }


    private void SetPauseMenuUIState(bool state)
    {
        pauseMenuUI.SetActive(state);
    }

    private void SetSettingsStateFalse()
    {
        isSettingsOpen = false;
        settingsPanel.SetActive(false);
    }

}