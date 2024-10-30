using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider mainVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider soundFXSlider;
    public void Start()
    {
        if (mainVolumeSlider != null)
        {
            mainVolumeSlider.value = FeedbackAnimationParameters.mainVolume;
            musicVolumeSlider.value = FeedbackAnimationParameters.musicVolume;
            soundFXSlider.value = FeedbackAnimationParameters.soundFXVolume;

            audioMixer.SetFloat("masterVolume", Mathf.Log10(mainVolumeSlider.value) * 20f);
            audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolumeSlider.value) * 20f);
            audioMixer.SetFloat("soundFXVolume", Mathf.Log10(soundFXSlider.value) * 20f);
        }
    }
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        FeedbackAnimationParameters.mainVolume = level;
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        FeedbackAnimationParameters.musicVolume = level;
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        FeedbackAnimationParameters.soundFXVolume = level;
    }
}
