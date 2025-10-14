using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool : EditorWindow
{
    private AudioSource previewAudioSource;
    private bool isPlaying = false;
    private float[] trimmedSamples;
    private bool isAudioAdded = false;

    private AudioClip audioClip;
    private float startTrim = 0f;
    private float endTrim = 0f;
    private float fadeStartDuration = 0f;
    private float fadeEndDuration = 0f;
    private bool loopPreview = false;
    
    private int numberOfMeasure = 0;
    private int measure = 0;
    private int beat = 0;
    private int division = 0;

    private bool[,,] sheetMusic; 

    private Texture2D waveformTexture;
    private const int waveformWidth = 500;
    private const int waveformHeight = 100;
    
    

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorTool>("Audio Editor");
    }

    private void OnEnable()
    {
        if (previewAudioSource == null)
        {
            // Create an audio source for previewing the sound
            GameObject audioPreviewer = new GameObject("AudioPreviewer");
            previewAudioSource = audioPreviewer.AddComponent<AudioSource>();
            previewAudioSource.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void OnDisable()
    {
        if (previewAudioSource != null)
        {
            DestroyImmediate(previewAudioSource.gameObject);
        }
    }

    private void OnGUI()
    {

        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);

        if (audioClip != null)
        {
            if (waveformTexture == null || GUILayout.Button("Generate Waveform"))
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0),
                    startTrim, endTrim, fadeStartDuration, fadeEndDuration);
            }
                
            numberOfMeasure = EditorGUILayout.IntField("Nombre de mesure : ", numberOfMeasure);
            
            measure = EditorGUILayout.IntField("Mesure actuelle : ", measure);
            
            beat = EditorGUILayout.IntField("Nombre de temps : ", beat);
            
            division = EditorGUILayout.IntField("Division par temps : ", division);

            if (sheetMusic == null || sheetMusic.GetLength(0) != numberOfMeasure ||
                    sheetMusic.GetLength(1) != beat || sheetMusic.GetLength(2) != division)
            {
                sheetMusic = ResizeSheetMusic(sheetMusic,numberOfMeasure, beat, division);
            }
            
            for (int i = 0; i < beat; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < division; j++)
                {
                    if (sheetMusic[measure, i,j])
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    sheetMusic[measure, i,j] = EditorGUILayout.Toggle(sheetMusic[measure, i,j]);
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }
            // quand on se met a la mesure maximale il y a un out of range
            // rajouter les petits batons des mesures en bas 
            // rajouter un son de preview (des petits clac a chaque rythme qu'on a placé)
            // permettre d'enregister et de load un niveau (enregistrer le tableau dans un scriptable peut être)
            // permettre de mettre les rythmes differents (et changer la couleur de la box en fonction du rythme (peut être changer le tableau de bool en tableau d'int))
            // rajouter une option ou on met le bpm et ça nous sors le nombre de mesure
            
            if (waveformTexture != null)
            {
                GUILayout.Label("Waveform Preview");
                GUILayout.Box(waveformTexture);

                if (isPlaying)
                {
                    Rect waveformRect = GUILayoutUtility.GetLastRect();
                    float playheadPosition =
                        Mathf.Min(
                            ((previewAudioSource.time) / (previewAudioSource.clip.length / 2f)) * waveformRect.width,
                            waveformRect.width);
                    Rect playheadRect = new Rect(playheadPosition, GUILayoutUtility.GetLastRect().y, 2, waveformHeight);
                    EditorGUI.DrawRect(playheadRect, Color.red);
                }

            }
            loopPreview = GUILayout.Toggle(loopPreview, "Loop Preview");
            if (GUI.changed)
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0),
                    startTrim, endTrim, fadeStartDuration, fadeEndDuration);
            }
            if (!isAudioAdded)
            {
                endTrim = audioClip.length;
                isAudioAdded = true;
            }
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Play"))
            {
                PlayPreview();
            }

            if (GUILayout.Button("Stop"))
            {
                StopPreview();
            }

            GUILayout.EndHorizontal();

        }
        else
        {
            isAudioAdded = false;
        }

        if (isPlaying)
        {
            Repaint();
        }
    }
    
    bool[,,] ResizeSheetMusic(bool[,,] oldSheet, int newMeasures, int newBeats, int newDivisions)
    {
        bool[,,] newSheet = new bool[newMeasures, newBeats, newDivisions];

        if (oldSheet == null)
            return newSheet;

        int minMeasures = Mathf.Min(oldSheet.GetLength(0), newMeasures);
        int minBeats = Mathf.Min(oldSheet.GetLength(1), newBeats);
        int minDivisions = Mathf.Min(oldSheet.GetLength(2), newDivisions);

        for (int m = 0; m < minMeasures; m++)
        for (int b = 0; b < minBeats; b++)
        for (int d = 0; d < minDivisions; d++)
            newSheet[m, b, d] = oldSheet[m, b, d];

        return newSheet;
    }


    private void PlayPreview()
    {
        if (previewAudioSource == null || audioClip == null)
            return;

        if (isPlaying)
        {
            StopPreview();
        }

        // Trim and fade the audio for preview
        trimmedSamples = TrimAndFadeAudioSamples(audioClip, startTrim, endTrim, fadeStartDuration, fadeEndDuration);
        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", trimmedSamples.Length, audioClip.channels,
            audioClip.frequency, false);
        trimmedClip.SetData(trimmedSamples, 0);
        previewAudioSource.loop = loopPreview;
        previewAudioSource.clip = trimmedClip;
        previewAudioSource.Play();

        isPlaying = true;
    }

    private void StopPreview()
    {
        if (previewAudioSource == null || !isPlaying)
            return;

        previewAudioSource.Stop();
        isPlaying = false;
    }

    private float[] TrimAndFadeAudioSamples(AudioClip clip, float startTrim, float endTrim, float fadeStartDuration,
        float fadeEndDuration)
    {
        int startSample = Mathf.FloorToInt(startTrim * clip.frequency * clip.channels);
        int endSample = Mathf.FloorToInt(endTrim * clip.frequency * clip.channels);
        int trimSamples = endSample - startSample;

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        float[] trimmedSamples = new float[trimSamples];
        Array.Copy(samples, startSample, trimmedSamples, 0, trimSamples);

        // Apply fade-in and fade-out
        int fadeInSampleCount = Mathf.FloorToInt(fadeStartDuration * clip.frequency * clip.channels);
        int fadeOutSampleCount = Mathf.FloorToInt(fadeEndDuration * clip.frequency * clip.channels);

        // Apply fade-in
        for (int i = 0; i < fadeInSampleCount && i < trimmedSamples.Length; i++)
        {
            float fadeFactor = (float)i / fadeInSampleCount;
            trimmedSamples[i] *= fadeFactor;
        }

        // Apply fade-out
        if (fadeEndDuration > 0)
        {

            // Ensure fade out does not exceed the length of the trimmed data
            int fadeOutStart = trimmedSamples.Length - fadeOutSampleCount;

            for (int i = 0; i < fadeOutSampleCount && i < trimmedSamples.Length; i++)
            {
                // Calculate the fade factor, which starts at 1 and decreases to 0
                float fadeFactor = 1f - ((float)i / fadeOutSampleCount);

                // Apply fade-out to the end of the trimmed data
                trimmedSamples[fadeOutStart + i] *= fadeFactor;
            }
        }

        return trimmedSamples;
    }

    private Texture2D DrawWaveform(AudioClip clip, int width, int height, Color waveformColor, float startTrim,
        float endTrim, float fadeStartDuration, float fadeEndDuration)
    {
        Texture2D texture = new Texture2D(width, height);
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0.2f, 0.2f, 0.2f); // Background color
        }

        // Calculate the range of samples based on trim
        int startSample = Mathf.FloorToInt(startTrim * clip.frequency * clip.channels);
        int endSample = Mathf.FloorToInt(endTrim * clip.frequency * clip.channels);
        int trimSamples = endSample - startSample;

        // Calculate fade-in and fade-out sample ranges
        int fadeInSampleCount = Mathf.FloorToInt(fadeStartDuration * clip.frequency * clip.channels);
        int fadeOutSampleCount = Mathf.FloorToInt(fadeEndDuration * clip.frequency * clip.channels);

        int packSize = (trimSamples / width) + 1; // Calculate packSize based on the trimmed range

        for (int i = 0; i < width; i++)
        {
            float max = 0;
            for (int j = 0; j < packSize; j++)
            {
                int index = startSample + (i * packSize) + j;
                if (index < samples.Length)
                {
                    float wavePeak = Mathf.Abs(samples[index]);
                    int currentSampleIndex = startSample + (i * packSize);
                    if (currentSampleIndex < startSample + fadeInSampleCount)
                    {
                        float fadeFactor = (float)(currentSampleIndex - startSample) / fadeInSampleCount;
                        wavePeak *= fadeFactor;
                    }
                    if (currentSampleIndex > endSample - fadeOutSampleCount)
                    {
                        float fadeFactor = (float)(endSample - currentSampleIndex) / fadeOutSampleCount;
                        wavePeak *= fadeFactor;
                    }

                    if (wavePeak > max) max = wavePeak;
                }
            }

            int heightPos = Mathf.FloorToInt(max * (height / 2));
            for (int j = 0; j < heightPos; j++)
            {
                colors[(height / 2 + j) * width + i] = waveformColor;
                colors[(height / 2 - j) * width + i] = waveformColor;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

}
