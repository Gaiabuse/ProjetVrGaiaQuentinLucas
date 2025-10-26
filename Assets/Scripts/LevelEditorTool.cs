using System;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

public class LevelEditorTool : EditorWindow
{
    private AudioSource previewAudioSource;
    private bool isPlaying = false;
    private AudioClip audioClip;
    public AudioClip noteSound;
    private bool loopPreview = false;
    private Rect clickArea;

    private int bpm = 90;
    private int numberOfMeasure = 20;
    private int measure = 0;
    private int beat = 4;
    private int division = 4;
    private int actualRythm = 1;

    private Color[] colors = new Color[5]
    {
        Color.cyan, 
        Color.red,
        Color.yellow, 
        Color.green, 
        Color.purple
    };
    private int[,,] sheetMusic;
    private Vector2[,,] spawnPositions;

    private Texture2D waveformTexture;
    private Vector3Int lastCheckedIndex = new Vector3Int(-1, -1, -1);


    private const int waveformWidth = 500;
    private const int waveformHeight = 100;
    private const int maxBpm = 250;
    private const int maxTimePerMeasure = 12;
    private const int maxDivision = 8;

    private int lastPlayedMeasure = -1;
    private int lastPlayedBeat = -1;
    private int lastPlayedDivision = -1;

    private LevelData levelData;
    private LevelData lastLoadedLevelData;

    private Vector2 actualPos;

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
            clickArea = new Rect(30, 460, 640, 360);
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
        noteSound = (AudioClip)EditorGUILayout.ObjectField("Note Sound", noteSound, typeof(AudioClip), false);

        LevelData newLevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        if (newLevelData != levelData)
        {
            levelData = newLevelData;
            if (levelData != null)
            {
                audioClip = levelData.audioClip;
                noteSound = levelData.noteSound;
                sheetMusic = levelData.sheetMusic;
                spawnPositions = levelData.spawnPositions;
                bpm = levelData.bpm;
                beat = levelData.beat;
                division = levelData.division;
                numberOfMeasure = (sheetMusic != null) ? sheetMusic.GetLength(0) : 0;
            }
        }

        if (audioClip != null)
        {
            

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save New Level"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save Level Data", "NewLevelData", "asset", "Choose a file name and location");
                if (!string.IsNullOrEmpty(path))
                {
                    LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
                    newLevel.audioClip = audioClip;
                    newLevel.noteSound = noteSound;
                    newLevel.sheetMusic = sheetMusic;
                    newLevel.spawnPositions = spawnPositions;
                    newLevel.bpm = bpm;
                    newLevel.beat = beat;
                    newLevel.division = division;
                    AssetDatabase.CreateAsset(newLevel, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    levelData = newLevel;
                }
            }

            if (levelData != null && GUILayout.Button("Update Level"))
            {
                levelData.audioClip = audioClip;
                levelData.noteSound = noteSound;
                levelData.sheetMusic = sheetMusic;
                levelData.spawnPositions = spawnPositions;
                levelData.bpm = bpm;
                levelData.beat = beat;
                levelData.division = division;
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();

            EditorGUI.DrawRect(clickArea, Color.black);

            float secondsPerBeat = 60f / bpm;
            float secondsPerMeasure = secondsPerBeat * beat;
            numberOfMeasure = Mathf.CeilToInt(audioClip.length / secondsPerMeasure);

            if (waveformTexture == null || GUILayout.Button("Generate Waveform"))
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0), numberOfMeasure);
            }
            
            actualRythm = EditorGUILayout.IntField("Index rythme actuel : ", actualRythm);
            actualRythm = Mathf.Clamp(actualRythm, 1, colors.Length);

            bpm = EditorGUILayout.IntField("Battement par minute : ", bpm);
            bpm = Mathf.Clamp(bpm, 1, maxBpm);

            measure = EditorGUILayout.IntField("Mesure actuelle : ", measure);
            measure = Mathf.Clamp(measure, 0, Mathf.Max(0, numberOfMeasure - 1));

            beat = EditorGUILayout.IntField("Nombre de temps : ", beat);
            beat = Mathf.Clamp(beat, 1, maxTimePerMeasure);

            division = EditorGUILayout.IntField("Division par temps : ", division);
            division = Mathf.Clamp(division, 1, maxDivision);

            if (sheetMusic == null || sheetMusic.GetLength(0) != numberOfMeasure || sheetMusic.GetLength(1) != beat || sheetMusic.GetLength(2) != division)
            {
                sheetMusic = ResizeSheetMusic(sheetMusic, numberOfMeasure, beat, division);
                spawnPositions = ResizeSheetMusic(spawnPositions, numberOfMeasure, beat, division);
            }

            for (int i = 0; i < beat; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < division; j++)
                {
                    int actualInt = sheetMusic[measure, i, j];
                    spawnPositions[measure, i, j].x = Mathf.Clamp(spawnPositions[measure, i, j].x, 0, clickArea.width - 20);
                    spawnPositions[measure, i, j].y = Mathf.Clamp(spawnPositions[measure, i, j].y, 0, clickArea.height - 20);
                    if (actualInt != 0)
                    {
                        EditorGUI.DrawRect(new Rect(clickArea.x + spawnPositions[measure, i, j].x, clickArea.y + clickArea.height - spawnPositions[measure, i, j].y - 20, 20, 20), colors[actualInt - 1]);
                        GUI.backgroundColor = colors[actualInt - 1]; 
                    }
                    float oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 20f;
                    GUILayout.Space(50);

                    bool isChecked = sheetMusic[measure, i, j] != 0;
                    bool newChecked = EditorGUILayout.Toggle(isChecked, GUILayout.Width(20));
                    if (newChecked != isChecked)
                    {
                        sheetMusic[measure, i, j] = newChecked ? actualRythm : 0;
                        if (newChecked)
                        {
                            lastCheckedIndex = new Vector3Int(measure, i, j);
                        }
                    }

                    spawnPositions[measure, i, j].x = EditorGUILayout.FloatField("x :", spawnPositions[measure, i, j].x, GUILayout.Width(50));
                    spawnPositions[measure, i, j].y = EditorGUILayout.FloatField("y :", spawnPositions[measure, i, j].y, GUILayout.Width(50));
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
                    float playheadPosition = Mathf.Min((previewAudioSource.time / previewAudioSource.clip.length) * waveformRect.width, waveformRect.width);
                    Rect playheadRect = new Rect(playheadPosition, waveformRect.y, 2, waveformHeight);
                    EditorGUI.DrawRect(playheadRect, Color.red);
                }
            }

            loopPreview = GUILayout.Toggle(loopPreview, "Loop Preview");

            if (GUI.changed)
            {
                waveformTexture = DrawWaveform(audioClip, waveformWidth, waveformHeight, new Color(1, 0.5f, 0), numberOfMeasure);
            }

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
            if (actualEvent.type == EventType.MouseDown && clickArea.Contains(actualEvent.mousePosition))
            {
                actualPos = actualEvent.mousePosition - new Vector2(clickArea.x, clickArea.y);
                actualPos.y = clickArea.height - actualPos.y;
                if (lastCheckedIndex.x >= 0)
                {
                    spawnPositions[lastCheckedIndex.x, lastCheckedIndex.y, lastCheckedIndex.z].x = actualPos.x;
                    spawnPositions[lastCheckedIndex.x, lastCheckedIndex.y, lastCheckedIndex.z].y = actualPos.y;
                }


            }
            GUILayout.Space(375);
            GUILayout.Label("Click position -> x : " + actualPos.x + " / y : "+ actualPos.y);
            
            if (isPlaying)
            {
                PlaySheetMusicNotes();
                Repaint();
            }
        }
    }


    int[,,] ResizeSheetMusic(int[,,] oldSheet, int newMeasures, int newBeats, int newDivisions)
    {
        int[,,] newSheet = new int[newMeasures, newBeats, newDivisions];
        if (oldSheet == null)
        {
            return newSheet;
        }
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
        {
            return newSheet;
        }
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
            StopPreview();

        previewAudioSource.loop = loopPreview;
        previewAudioSource.clip = audioClip;
        previewAudioSource.Play();

        isPlaying = true;
        lastPlayedMeasure = -1;
        lastPlayedBeat = -1;
        lastPlayedDivision = -1;
    }

    private void StopPreview()
    {
        if (previewAudioSource == null || !isPlaying)
            return;

        previewAudioSource.Stop();
        isPlaying = false;
    }

    private void PlaySheetMusicNotes()
    {
        if (!isPlaying || audioClip == null || noteSound == null) return;

        float totalTime = previewAudioSource.time;
        float secondsPerBeat = 60f / bpm;
        float secondsPerMeasure = secondsPerBeat * beat;
        float secondsPerDivision = secondsPerBeat / division;

        for (int m = 0; m < numberOfMeasure; m++)
        {
            for (int b = 0; b < beat; b++)
            {
                for (int d = 0; d < division; d++)
                {
                    float divisionStartTime = m * secondsPerMeasure + b * secondsPerBeat + d * secondsPerDivision;

                    bool alreadyPlayed = false;
                    if (m < lastPlayedMeasure) alreadyPlayed = true;
                    else if (m == lastPlayedMeasure && b < lastPlayedBeat) alreadyPlayed = true;
                    else if (m == lastPlayedMeasure && b == lastPlayedBeat && d <= lastPlayedDivision) alreadyPlayed = true;

                    if (!alreadyPlayed && sheetMusic[m, b, d] != 0 && divisionStartTime <= totalTime)
                    {
                        previewAudioSource.PlayOneShot(noteSound);
                        lastPlayedMeasure = m;
                        lastPlayedBeat = b;
                        lastPlayedDivision = d;
                    }
                }
            }
        }
    }




    private Texture2D DrawWaveform(AudioClip clip, int width, int height, Color waveformColor, int measures)
    {
        Texture2D texture = new Texture2D(width, height);
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(0.2f, 0.2f, 0.2f);

        int packSize = Mathf.Max(1, samples.Length / width);
        for (int i = 0; i < width; i++)
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
                int x = Mathf.FloorToInt((float)m / measures * width);
                if (x >= width) continue;
                Color actualColor = (m == measure) ? Color.white : Color.black;
                for (int y = 0; y < height; y++)
                {
                    colors[y * width + x] = actualColor;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }


}
