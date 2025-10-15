using System;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool : EditorWindow
{
    private AudioSource previewAudioSource;
    private bool isPlaying = false;
    private bool isAudioAdded = false;

    private AudioClip audioClip;
    private bool loopPreview = false;

    private int numberOfMeasure = 20;
    private int measure = 0;
    private int beat = 4;
    private int division = 4;

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
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0), numberOfMeasure);
            }

            numberOfMeasure = EditorGUILayout.IntField("Nombre de mesure : ", numberOfMeasure);
            measure = EditorGUILayout.IntField("Mesure actuelle : ", measure);
            beat = EditorGUILayout.IntField("Nombre de temps : ", beat);
            division = EditorGUILayout.IntField("Division par temps : ", division);

            if (sheetMusic == null || sheetMusic.GetLength(0) != numberOfMeasure ||
                sheetMusic.GetLength(1) != beat || sheetMusic.GetLength(2) != division)
            {
                sheetMusic = ResizeSheetMusic(sheetMusic, numberOfMeasure, beat, division);
            }

            for (int i = 0; i < beat; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < division; j++)
                {
                    if (sheetMusic[measure, i, j])
                    {
                        GUI.backgroundColor = Color.cyan;
                    }
                    sheetMusic[measure, i, j] = EditorGUILayout.Toggle(sheetMusic[measure, i, j]);
                    GUI.backgroundColor = Color.white;
                }
                GUILayout.EndHorizontal();
            }

            if (waveformTexture != null)
            {
                GUILayout.Label("Waveform Preview");
                GUILayout.Box(waveformTexture);

                if (isPlaying)
                {
                    Rect waveformRect = GUILayoutUtility.GetLastRect();
                    float playheadPosition = Mathf.Min(
                        (previewAudioSource.time / previewAudioSource.clip.length) * waveformRect.width,
                        waveformRect.width);
                    Rect playheadRect = new Rect(playheadPosition, waveformRect.y, 2, waveformHeight);
                    EditorGUI.DrawRect(playheadRect, Color.red);
                }
            }

            loopPreview = GUILayout.Toggle(loopPreview, "Loop Preview");

            if (GUI.changed)
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0), numberOfMeasure);
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

        previewAudioSource.loop = loopPreview;
        previewAudioSource.clip = audioClip;
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

    private Texture2D DrawWaveform(AudioClip clip, int width, int height, Color waveformColor, int measures)
    {
        Texture2D texture = new Texture2D(width, height);
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(0.2f, 0.2f, 0.2f);

        int usedWidth = Mathf.Min(width, Mathf.CeilToInt((float)clip.samples / (clip.frequency * clip.length) * width));

        int packSize = Mathf.Max(1, samples.Length / width);
        for (int i = 0; i < usedWidth; i++)
        {
            float max = 0;
            for (int j = 0; j < packSize; j++)
            {
                int index = (i * packSize) + j;
                if (index < samples.Length)
                {
                    float wavePeak = Mathf.Abs(samples[index]);
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

        if (measures > 1)
        {
            for (int m = 0; m <= measures; m++)
            {
                int x = Mathf.FloorToInt((float)m / measures * usedWidth);
                if (x >= usedWidth) continue;
                for (int y = 0; y < height; y++)
                {
                    int index = y * width + x;
                    colors[index] = Color.black;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}
