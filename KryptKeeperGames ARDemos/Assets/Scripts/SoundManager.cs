﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip poof_sfx;

    public AudioClip[] canClink_sfx;

    AudioSource audioSource;


    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip soundToPlay)
    {
        audioSource.PlayOneShot(soundToPlay);
    }
}

