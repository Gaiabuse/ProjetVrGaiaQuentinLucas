using Data;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

#if UNITY_EDITOR
namespace Editor
{
    public class LevelEditorTool : EditorWindow
    {
        private AudioSource _previewAudioSource;
        private bool _isPlaying = false;
        private AudioClip _audioClip;
        private AudioClip _noteSound;
        private bool _loopPreview = false;
        private Rect _clickArea;

        private int _bpm = 90;
        private int _numberOfMeasure = 20;
        private int _measure = 0;
        private int _beat = 4;
        private int _division = 4;
        private int _actualRythm = 1;

        private Color[] _colors = new Color[5]
        {
            Color.cyan, 
            Color.red,
            Color.yellow, 
            Color.green, 
            Color.purple
        };
        private int[,,] _sheetMusic;
        private Vector2[,,] _spawnPositions;

        private Texture2D _waveformTexture;
        private Vector3Int _lastCheckedIndex = new Vector3Int(-1, -1, -1);

        private const int _waveformWidth = 500;
        private const int _waveformHeight = 100;
        private const int _maxBpm = 250;
        private const int _maxTimePerMeasure = 12;
        private const int _maxDivision = 8;

        private int _lastPlayedMeasure = -1;
        private int _lastPlayedBeat = -1;
        private int _lastPlayedDivision = -1;

        private LevelData _levelData;
        private LevelData _lastLoadedLevelData;

        private Vector2 _actualPos;

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
            if (_previewAudioSource == null)
            {
                _clickArea = new Rect(30, 450, 640, 360);
                GameObject audioPreviewer = new GameObject("AudioPreviewer");
                _previewAudioSource = audioPreviewer.AddComponent<AudioSource>();
                _previewAudioSource.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private void OnDisable()
        {
            if (_previewAudioSource != null)
            {
                DestroyImmediate(_previewAudioSource.gameObject);
            }
        }

        private void OnGUI()
        {
            _audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", _audioClip, typeof(AudioClip), false);
            _noteSound = (AudioClip)EditorGUILayout.ObjectField("Note Sound", _noteSound, typeof(AudioClip), false);

            LevelData newLevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", _levelData, typeof(LevelData), false);

            if (newLevelData != _levelData)
            {
                _levelData = newLevelData;
                if (_levelData != null)
                {
                    _audioClip = _levelData.audioClip;
                    _noteSound = _levelData.noteSound;
                    _sheetMusic = _levelData.sheetMusic;
                    _spawnPositions = _levelData.spawnPositions;
                    _bpm = _levelData.bpm;
                    _beat = _levelData.beat;
                    _division = _levelData.division;
                    _numberOfMeasure = (_sheetMusic != null) ? _sheetMusic.GetLength(0) : 0;
                }
            }

            if (_audioClip != null)
            {
            

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save New Level"))
                {
                    string path = EditorUtility.SaveFilePanelInProject("Save Level Data", "NewLevelData", "asset", "Choose a file name and location");
                    if (!string.IsNullOrEmpty(path))
                    {
                        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
                        newLevel.audioClip = _audioClip;
                        newLevel.noteSound = _noteSound;
                        newLevel.sheetMusic = _sheetMusic;
                        newLevel.spawnPositions = _spawnPositions;
                        newLevel.bpm = _bpm;
                        newLevel.beat = _beat;
                        newLevel.division = _division;
                        AssetDatabase.CreateAsset(newLevel, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        _levelData = newLevel;
                    }
                }

                if (_levelData != null && GUILayout.Button("Update Level"))
                {
                    _levelData.audioClip = _audioClip;
                    _levelData.noteSound = _noteSound;
                    _levelData.sheetMusic = _sheetMusic;
                    _levelData.spawnPositions = _spawnPositions;
                    _levelData.bpm = _bpm;
                    _levelData.beat = _beat;
                    _levelData.division = _division;
                    EditorUtility.SetDirty(_levelData);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                GUILayout.EndHorizontal();

                EditorGUI.DrawRect(_clickArea, Color.black);

                float secondsPerBeat = 60f / _bpm;
                float secondsPerMeasure = secondsPerBeat * _beat;
                _numberOfMeasure = Mathf.CeilToInt(_audioClip.length / secondsPerMeasure);
            
                _actualRythm = EditorGUILayout.IntField("Index rythme actuel : ", _actualRythm);
                _actualRythm = Mathf.Clamp(_actualRythm, 1, _colors.Length);

                _bpm = EditorGUILayout.IntField("Battement par minute : ", _bpm);
                _bpm = Mathf.Clamp(_bpm, 1, _maxBpm);

                _measure = EditorGUILayout.IntField("Mesure actuelle : ", _measure);
                _measure = Mathf.Clamp(_measure, 0, Mathf.Max(0, _numberOfMeasure - 1));

                _beat = EditorGUILayout.IntField("Nombre de temps : ", _beat);
                _beat = Mathf.Clamp(_beat, 1, _maxTimePerMeasure);

                _division = EditorGUILayout.IntField("Division par temps : ", _division);
                _division = Mathf.Clamp(_division, 1, _maxDivision);

                if (_sheetMusic == null || _sheetMusic.GetLength(0) != _numberOfMeasure || _sheetMusic.GetLength(1) != _beat || _sheetMusic.GetLength(2) != _division)
                {
                    _sheetMusic = ResizeSheetMusic(_sheetMusic, _numberOfMeasure, _beat, _division);
                    _spawnPositions = ResizeSheetMusic(_spawnPositions, _numberOfMeasure, _beat, _division);
                }

                for (int i = 0; i < _beat; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < _division; j++)
                    {
                        int actualInt = _sheetMusic[_measure, i, j];
                        _spawnPositions[_measure, i, j].x = Mathf.Clamp(_spawnPositions[_measure, i, j].x, 0, _clickArea.width - 20);
                        _spawnPositions[_measure, i, j].y = Mathf.Clamp(_spawnPositions[_measure, i, j].y, 0, _clickArea.height - 20);
                        if (actualInt != 0)
                        {
                            EditorGUI.DrawRect(new Rect(_clickArea.x + _spawnPositions[_measure, i, j].x, _clickArea.y + _clickArea.height - _spawnPositions[_measure, i, j].y - 20, 20, 20), _colors[actualInt - 1]);
                            GUI.backgroundColor = _colors[actualInt - 1]; 
                        }
                        float oldWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 20f;
                        GUILayout.Space(50);

                        bool isChecked = _sheetMusic[_measure, i, j] != 0;
                        bool newChecked = EditorGUILayout.Toggle(isChecked, GUILayout.Width(20));
                        if (newChecked != isChecked)
                        {
                            _sheetMusic[_measure, i, j] = newChecked ? _actualRythm : 0;
                            if (newChecked)
                            {
                                _lastCheckedIndex = new Vector3Int(_measure, i, j);
                            }
                        }

                        _spawnPositions[_measure, i, j].x = EditorGUILayout.FloatField("x :", _spawnPositions[_measure, i, j].x, GUILayout.Width(50));
                        _spawnPositions[_measure, i, j].y = EditorGUILayout.FloatField("y :", _spawnPositions[_measure, i, j].y, GUILayout.Width(50));
                        EditorGUIUtility.labelWidth = oldWidth;
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(2);
                    }
                    GUILayout.EndHorizontal();
                }

                if (_waveformTexture != null)
                {
                    GUILayout.Label("Waveform");
                    GUILayout.Box(_waveformTexture);

                    if (_isPlaying)
                    {
                        Rect waveformRect = GUILayoutUtility.GetLastRect();
                        float playheadPosition = Mathf.Min((_previewAudioSource.time / _previewAudioSource.clip.length) * waveformRect.width, waveformRect.width);
                        Rect playheadRect = new Rect(playheadPosition, waveformRect.y, 2, _waveformHeight);
                        EditorGUI.DrawRect(playheadRect, Color.red);
                    }
                }

                _loopPreview = GUILayout.Toggle(_loopPreview, "Loop");

                if (GUI.changed)
                {
                    _waveformTexture = DrawWaveform(_audioClip, _waveformWidth, _waveformHeight, new Color(1, 0.5f, 0), _numberOfMeasure);
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
                if (actualEvent.type == EventType.MouseDown && _clickArea.Contains(actualEvent.mousePosition))
                {
                    _actualPos = actualEvent.mousePosition - new Vector2(_clickArea.x, _clickArea.y);
                    _actualPos.y = _clickArea.height - _actualPos.y;
                    if (_lastCheckedIndex.x >= 0)
                    {
                        _spawnPositions[_lastCheckedIndex.x, _lastCheckedIndex.y, _lastCheckedIndex.z].x = _actualPos.x;
                        _spawnPositions[_lastCheckedIndex.x, _lastCheckedIndex.y, _lastCheckedIndex.z].y = _actualPos.y;
                    }


                }
                GUILayout.Space(385);
                GUILayout.Label("Click position -> x : " + _actualPos.x + " / y : "+ _actualPos.y);
            
                if (_isPlaying)
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
            if (_previewAudioSource == null || _audioClip == null)
                return;

            if (_isPlaying)
                StopPreview();

            _previewAudioSource.loop = _loopPreview;
            _previewAudioSource.clip = _audioClip;
            _previewAudioSource.Play();

            _isPlaying = true;
            _lastPlayedMeasure = -1;
            _lastPlayedBeat = -1;
            _lastPlayedDivision = -1;
        }

        private void StopPreview()
        {
            if (_previewAudioSource == null || !_isPlaying)
                return;

            _previewAudioSource.Stop();
            _isPlaying = false;
        }

        private void PlaySheetMusicNotes()
        {
            if (!_isPlaying || _audioClip == null || _noteSound == null) return;

            float totalTime = _previewAudioSource.time;
            float secondsPerBeat = 60f / _bpm;
            float secondsPerMeasure = secondsPerBeat * _beat;
            float secondsPerDivision = secondsPerBeat / _division;

            for (int m = 0; m < _numberOfMeasure; m++)
            {
                for (int b = 0; b < _beat; b++)
                {
                    for (int d = 0; d < _division; d++)
                    {
                        float divisionStartTime = m * secondsPerMeasure + b * secondsPerBeat + d * secondsPerDivision;

                        bool alreadyPlayed = false;
                        if (m < _lastPlayedMeasure) alreadyPlayed = true;
                        else if (m == _lastPlayedMeasure && b < _lastPlayedBeat) alreadyPlayed = true;
                        else if (m == _lastPlayedMeasure && b == _lastPlayedBeat && d <= _lastPlayedDivision) alreadyPlayed = true;

                        if (!alreadyPlayed && _sheetMusic[m, b, d] != 0 && divisionStartTime <= totalTime)
                        {
                            _previewAudioSource.PlayOneShot(_noteSound);
                            _lastPlayedMeasure = m;
                            _lastPlayedBeat = b;
                            _lastPlayedDivision = d;
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
                    Color actualColor = (m == _measure) ? Color.white : Color.black;
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
}

#endif