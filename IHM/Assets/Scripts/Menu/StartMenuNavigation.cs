using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public Button startButton;  // The button to be selected by default
    public Button quitButton;
    public Button settingsButton;
    public GameObject settingsPanel;
    public Button settingsReturnButton;

    private GameObject previousSelectedButton;
    private Button selectedButton;
    private bool isSettingsOpen = false; // Flag to track if settings are open

    private void Start()
    {
        // Automatically select the Start button when the menu loads
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        startButton.onClick.AddListener(LoadLevel);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(LoadSettings);
        settingsReturnButton.onClick.AddListener(CloseSettings);

        settingsPanel.SetActive(false);
    }

    private void Update()
    {


        // Detect A button (Gamepad) or Enter key (Keyboard) for button activation
        if (!isSettingsOpen && (Keyboard.current.enterKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)))
        {
            selectedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
            if (selectedButton != null)
            {
                selectedButton.onClick.Invoke();
            }
        }
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
            Debug.Log("Load Settings Pressed");

            // Store the currently selected button in the main menu
            previousSelectedButton = EventSystem.current.currentSelectedGameObject;

            // Show the pop-up panel
            settingsPanel.SetActive(true);

            SetStartMenuButtonsState(false);
            Debug.Log("Settings Panel Activated");

            

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
            Debug.Log("Closing Settings");

            // Hide the settings panel
            settingsPanel.SetActive(false);

            SetStartMenuButtonsState(true);

            Invoke("SetCloseSettingsBoolFalse", 0.05f);
            // Restore the previously selected button in the main menu
            if (previousSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
            }
            else
            {
                // Fallback to the default main menu button if no previous selection exists
                EventSystem.current.SetSelectedGameObject(settingsButton.gameObject); // Assuming startButton is the default
            }

            
        }
    }

    private void SetCloseSettingsBoolFalse()
    {
        isSettingsOpen = false;
    }

    private void SetStartMenuButtonsState(bool state)
    {
        startButton.gameObject.SetActive(state);
        settingsButton.gameObject.SetActive(state);
        quitButton.gameObject.SetActive(state);
    }

}