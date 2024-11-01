using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Text healthText;



    // Update is called once per frame
    void Update()
    {
        healthText.text = " x " + FeedbackAnimationParameters.health;
    }
}
