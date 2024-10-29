using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpTextController : MonoBehaviour
{

    public GameObject helpPanel;

    // Start is called before the first frame update
    void Start()
    {
        helpPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || ((Gamepad.current != null) && Gamepad.current.buttonWest.wasPressedThisFrame))
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }
}
