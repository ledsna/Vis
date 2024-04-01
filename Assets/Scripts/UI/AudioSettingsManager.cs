using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }

        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMusicVolume()
    {
        float volume = Mathf.Log10(musicSlider.value) * 20;
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    public void SetSFXVolume()
    {
        float volume = Mathf.Log10(soundFXSlider.value) * 20;
        audioMixer.SetFloat("soundFXVolume", volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void LoadSFXVolume()
    {
        soundFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }
}
