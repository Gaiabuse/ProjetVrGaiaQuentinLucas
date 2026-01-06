using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource voiceSource; // Source dédiée aux voix
    [SerializeField] private int sfxPoolSize = 10;
    
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        // Initialisation du pool d'effets sonores
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            _sfxPool.Add(source);
        }
    }

    // Joue un son ponctuel (SFX)
    public void PlaySfx(AudioClip sound, Transform attachment = null)
    {
        AudioSource source = GetFreeSource();
        ApplySettings(source, sound, attachment);
        source.Play();
    }

    // Joue une voix et retourne sa durée (pour tes coroutines)
    public float PlayVoice(AudioClip sound, Transform attachment = null)
    {
        ApplySettings(voiceSource, sound, attachment);
        voiceSource.Play();
        return sound.length;
    }

    public void StopVoice() => voiceSource.Stop();
    public bool IsVoicePlaying() => voiceSource.isPlaying;

    private void ApplySettings(AudioSource source, AudioClip sound, Transform target)
    {
        source.clip = sound;
        if (target != null) source.transform.position = target.position;
    }

    private AudioSource GetFreeSource()
    {
        return _sfxPool.Find(s => !s.isPlaying) ?? _sfxPool[0];
    }
}