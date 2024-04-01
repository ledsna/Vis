using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;
    
    private float lastStepTime;
    public float stepRate = 0.4f; // Time between steps

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayStepSoundFX()
    {
        if (Time.time - lastStepTime > stepRate)
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.stepSFX);
            lastStepTime = Time.time;
        }
    }

    public void PlayJumpSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.jumpSFX);
    }

    public void PlayLandSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.landSFX);
    }

    public void PlayDeathSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);
    }
}
