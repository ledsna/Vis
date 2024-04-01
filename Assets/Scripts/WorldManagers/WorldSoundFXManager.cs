using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Music")] 
    private AudioSource musicAudioSource;
    public AudioClip backgroundMusic;

    [Header("Player SFX")]
    public AudioClip stepSFX;
    public AudioClip jumpSFX;
    public AudioClip landSFX;
    public AudioClip deathSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

        musicAudioSource.clip = backgroundMusic;
        musicAudioSource.Play();
    }
}
