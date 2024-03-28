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
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMusicVolume()
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicSlider.value) * 20);
    }

    public void SetSFXVolume()
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(soundFXSlider.value) * 20);
    }

}
