using System;
using System.Collections;
using UnityEngine;

public class MetronomeManager : MonoBehaviour 
{
    public AudioSource audioSource;
    
    [SerializeField] private AudioSource audioSourceMetronome;
    [SerializeField] private AudioClip firstMetronome; 
    [SerializeField] private AudioClip otherMetronome;
    [SerializeField] private int timeSignature = 4;
    [SerializeField] private float initialBpm = 120;

    private int division = 1;
    private int beatCpt = 0;
    private int divisionCpt = 0;
    private float actualTimeBetweenBeat;

    private bool isFirstMeasure = true;
    private bool canPlay = false;
    
    private int actualMeasure = 0;
    private int actualBeat = 0;
    private int actualDivision = 0;
    

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            return;
        }
        initialBpm = Mathf.Clamp(initialBpm, 40, 260);
        float secondsPerBeat = 60f / initialBpm;
        float secondsPerDivision = (60f / initialBpm)/division;
        int currentBeat = Mathf.FloorToInt(audioSource.time / secondsPerBeat) % timeSignature + 1;
        int currentDivision = Mathf.FloorToInt(audioSource.time / secondsPerDivision) % timeSignature + 1;
        if (currentBeat != beatCpt)
        {
            beatCpt = currentBeat;
            if (beatCpt == 1)
            {
                if (!isFirstMeasure)
                {
                    actualMeasure++;
                }
                else
                {
                    isFirstMeasure = false;
                }
                actualBeat = 0;
                audioSourceMetronome.PlayOneShot(firstMetronome);
            }
            else
            {
                actualBeat++;
                audioSourceMetronome.PlayOneShot(otherMetronome);
            }
            actualDivision = 0;
        }
        else if (currentDivision != divisionCpt )
        {
            divisionCpt = currentDivision;
            audioSourceMetronome.PlayOneShot(otherMetronome);
            actualDivision++;
            FightManager.INSTANCE.NoteSpawn(actualMeasure, actualBeat, actualDivision - 1);
            FightManager.INSTANCE.NotePrevisualisation(actualMeasure, actualBeat, actualDivision - 1);
        }
    }

    public void ChangeValues(AudioClip newSong, float newBpm = 120, int newTimeSignature = 4, int newDivision = 1)
    {
        audioSource.clip = newSong;
        initialBpm = newBpm;
        timeSignature = newTimeSignature;
        division = newDivision;
    }

    public void EndFight()
    {
        audioSource.Stop();
        audioSourceMetronome.Stop();
    }
    
}

