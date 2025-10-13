using System;
using System.Collections;
using UnityEngine;

public class MetronomeManager : MonoBehaviour // faire bpm et un moyen de caler les notes sur le bpm (on mettra un générateur de note ensuite)
{
    [SerializeField] private AudioSource audioSourceMetronome;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip firstMetronome; 
    [SerializeField] private AudioClip otherMetronome;
    [SerializeField] private float initialBpm = 120;
    [SerializeField] private int timeSignature = 4;
    
    private int cpt = 0;
    private float actualTimeBetweenBeat;
    private float lastBeatTime;

    

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            return;
        }
        initialBpm = Mathf.Clamp(initialBpm, 40, 260);
        float secondsPerBeat = 60f / initialBpm;
        int currentBeat = Mathf.FloorToInt(audioSource.time / secondsPerBeat) % timeSignature + 1;
        if (currentBeat != cpt)
        {
            cpt = currentBeat;

            if (cpt == 1)
            {
                audioSourceMetronome.PlayOneShot(firstMetronome);
            }
            else
            {
                audioSourceMetronome.PlayOneShot(otherMetronome);
            }
            lastBeatTime = audioSource.time;
        }
    }

    private void ChangeBpm()
    {
        // faire changer le bpm
    }
}

