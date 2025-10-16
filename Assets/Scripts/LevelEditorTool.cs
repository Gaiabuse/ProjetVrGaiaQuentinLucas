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
    
    private Rect clickArea ;

    private int numberOfMeasure = 20;
    private int measure = 0;
    private int beat = 4;
    private int division = 4;
    
    private bool[,,] sheetMusic;
    private Vector2[,,] spawnPositions;

    private Texture2D waveformTexture;
    private const int waveformWidth = 500;
    private const int waveformHeight = 100;
    private const int maxMeasure = 1000;
    private const int maxTimePerMeasure = 12;
    private const int maxDivision = 8;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorTool>("Audio Editor");
    }

    private void OnEnable()
    {
        LevelEditorTool window = GetWindow<LevelEditorTool>();
        window.minSize = new Vector2(700, 850);
        window.maxSize = new Vector2(1920, 1080);
        if (previewAudioSource == null)
        {
            clickArea = new Rect(30, 400, 640, 360);
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
            EditorGUI.DrawRect(clickArea , Color.black);
            
            if (waveformTexture == null || GUILayout.Button("Generate Waveform"))
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0), numberOfMeasure);
            }

            numberOfMeasure = EditorGUILayout.IntField("Nombre de mesure : ", numberOfMeasure);
            numberOfMeasure = Mathf.Clamp(numberOfMeasure, 1,maxMeasure);
            
            measure = EditorGUILayout.IntField("Mesure actuelle : ", measure);
            measure = Mathf.Clamp(measure, 0, numberOfMeasure - 1);
            
            beat = EditorGUILayout.IntField("Nombre de temps : ", beat);
            beat = Mathf.Clamp(beat, 1, maxTimePerMeasure);
            
            division = EditorGUILayout.IntField("Division par temps : ", division);
            division = Mathf.Clamp(division, 1, maxDivision);

            if (sheetMusic == null || sheetMusic.GetLength(0) != numberOfMeasure ||
                sheetMusic.GetLength(1) != beat || sheetMusic.GetLength(2) != division)
            {
                sheetMusic = ResizeSheetMusic(sheetMusic, numberOfMeasure, beat, division);
                spawnPositions = ResizeSheetMusic(spawnPositions, numberOfMeasure, beat, division);
            }

            for (int i = 0; i < beat; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < division; j++)
                {
                    if (sheetMusic[measure, i, j])
                    {
                        GUI.backgroundColor = Color.cyan;
                        EditorGUI.DrawRect(new Rect(clickArea.position.x + spawnPositions[measure,i,j].x , clickArea.position.y + clickArea.height - spawnPositions[measure,i,j].y - 20, 20, 20),Color.cyan);
                    }
                    float oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 20f;
                    GUILayout.Space(50);
                    sheetMusic[measure, i, j] = EditorGUILayout.Toggle(sheetMusic[measure, i, j]);
                    spawnPositions[measure, i, j].x =
                        EditorGUILayout.FloatField("x :", spawnPositions[measure, i, j].x, GUILayout.Width(50));
                    spawnPositions[measure, i, j].y =
                        EditorGUILayout.FloatField("y :", spawnPositions[measure, i, j].y, GUILayout.Width(50));
                    EditorGUIUtility.labelWidth = oldWidth;
                    GUI.backgroundColor = Color.white;
                    GUILayout.Space(2);
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
            
            
            
            Event actualEvent = Event.current;
            if (actualEvent.type == EventType.MouseDown&& clickArea.Contains(actualEvent.mousePosition))
            {
                Vector2 localPos = actualEvent.mousePosition - new Vector2(clickArea.x, clickArea.y);
                localPos.y = clickArea.height - localPos.y;
            }
            
            
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
    Vector2[,,] ResizeSheetMusic(Vector2[,,] oldSheet, int newMeasures, int newBeats, int newDivisions)
    {
        Vector2[,,] newSheet = new Vector2[newMeasures, newBeats, newDivisions];
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
            Color actualColor = Color.black;
            for (int m = 0; m <= measures; m++)
            {
                int x = Mathf.FloorToInt((float)m / measures * usedWidth);
                if (x >= usedWidth) continue;
                if (measure == m)
                {
                    actualColor = Color.white;
                }
                else
                {
                    actualColor = Color.black;
                }
                for (int y = 0; y < height; y++)
                {
                    int index = y * width + x;
                    if (measures == y)
                    {
                        colors[index] = actualColor;
                    }
                    else
                    {
                        colors[index] = actualColor;
                    }
                    
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}
