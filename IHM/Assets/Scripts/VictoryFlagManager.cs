using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryFlagManager : MonoBehaviour
{

    public AudioClip flagSoundClip;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponent<PlayerController>() != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(flagSoundClip, transform, 1f);
            Invoke("StartTransitionAnimation", 0.5f);
            Invoke("LoadVictoryMenu", 2f);

        }
    }

    private void StartTransitionAnimation()
    {
        FeedbackAnimationParameters.sceneTransition = true;
    }

    private void LoadVictoryMenu()
    {
        SceneManager.LoadScene("VictoryMenu");
    }
}
