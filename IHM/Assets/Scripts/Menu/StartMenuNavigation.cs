using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; // Input System for detecting A button
using UnityEngine.UI;         // UI for handling Button components
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public Button startButton;  // The button to be selected by default

    private void Start()
    {
        // Automatically select the Start button when the menu loads
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    private void Update()
    {
        // If no button is selected, ensure the Start button is selected by default
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }

        // Detect A button (Gamepad) or Enter key (Keyboard) for button activation
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            // Invoke the current button's action
            Button selectedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
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

    // Method to load the Settings scene
    public void LoadSettings()
    {
        SceneManager.LoadScene("settings");
    }

    // Method to quit the application
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // This will log in the console when quitting
        Application.Quit(); // Quit the application
    }
}
