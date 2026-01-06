using System;
using System.Collections;
using Data;
using Exploration;
using Fight;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class FightManager : MonoBehaviour
{
    public static FightManager INSTANCE;
    
    public static Action<bool> FightEnded;
    
    [SerializeField] private float maxAnxiety = 100f;
    [SerializeField] private MetronomeManager metronome;
    [SerializeField] private GameObject[] notesPrefabs;
    [SerializeField] private GameObject[] previewNotesPrefabs;
    [SerializeField] private LevelData level;
    [SerializeField] private float spawnPosDivider = 100f;
    [SerializeField] private float zAxisPosition = 0.2f;
    [SerializeField] private float zAxisPreviewOffset = 10f;
    [SerializeField] private Slider slider;
    [SerializeField] private int maxLinkedTime = 6;
    
    
    private Vector2[,,] _spawnPositions;
    private int[,,] _sheetMusic;
    private float _anxiety = 0f;
    private bool _canLink = false;
    private bool _inLine = true;
    private bool _isFirstLinkedNote = true;
    private InputDevice _leftHand;
    private InputDevice _rightHand;
    
    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Start / End
    
    public void StartFight(LevelData newLevel)
    {
        StartCoroutine(WaitForStartMusic());
        level = newLevel;
        _spawnPositions = level.SpawnPositions;
        _sheetMusic = level.SheetMusic;
        metronome.ChangeValues(level.AudioClip, level.Bpm, level.Beat, level.Division);
    }
    

    IEnumerator WaitForStartMusic()
    {
        yield return new WaitForSeconds(3f);
        metronome.AudioSourceMusic.Play();
    }

    public void EndFight(bool win)
    {
        _anxiety = 0f;
        PlayerManager.INSTANCE.AddLevelData(level,win);
        metronome.EndFight();
        FightEnded.Invoke(win);
    }
    
    #endregion
    
    
    public void AddAnxiety(float value)
    {
        _anxiety += value;
        slider.value = _anxiety;
        CheckLoose();
    }

	public LevelData GetLevel()
	{
		return level;
	}

    void CheckLoose()
    {
        if (_anxiety >= maxAnxiety)
        {
            EndFight(false);
        }
    }
    
    public void NoteSpawn(int actualMeasure, int actualBeat, int actualDivision)
    {
        int actualNote = _sheetMusic[actualMeasure, actualBeat, actualDivision];
        
        if (actualNote > notesPrefabs.Length)
        {
            return;
        }
        
        Vector2 actualPos = _spawnPositions[actualMeasure, actualBeat, actualDivision];

        if (actualNote != 0)
        {
            GameObject actualGO = Instantiate(notesPrefabs[actualNote - 1]);
            actualGO.transform.position = new Vector3(actualPos.x / spawnPosDivider, actualPos.y / spawnPosDivider, zAxisPosition);
            if (actualNote == 2)
            {
                LinkedNotes linked = actualGO.GetComponent<LinkedNotes>();
                linked.ChangeSheetMusicPosition(new Vector3Int(actualMeasure, actualBeat, actualDivision));
            }
        }
    }
    
    public void NotePrevisualisation(int actualMeasure, int actualBeat, int actualDivision)
    {
        actualMeasure += 1;
        
        int actualPreviewNote = _sheetMusic[actualMeasure, actualBeat, actualDivision];
        
        if (actualPreviewNote > previewNotesPrefabs.Length)
        {
            return;
        }
        
        Vector2 actualPos = _spawnPositions[actualMeasure, actualBeat, actualDivision];
        
        if (actualPreviewNote != 0)
        {
            if (actualPreviewNote == 2 )
            {
                if (_canLink)
                {
                    return;
                }

                CanLinkState(true);

            }
            GameObject actualGO = Instantiate(previewNotesPrefabs[actualPreviewNote - 1]);
            actualGO.transform.position = new Vector3(actualPos.x / spawnPosDivider, actualPos.y / spawnPosDivider, zAxisPosition + zAxisPreviewOffset);
        }
    }

    public int GetNote(int measure, int beat, int division)
    {
        return level.SheetMusic[measure, beat, division];
    }
    public Vector3 GetPos(int measure, int beat, int division)
    {
        return new Vector3(level.SpawnPositions[measure, beat, division].x / spawnPosDivider,level.SpawnPositions[measure, beat, division].y / spawnPosDivider, zAxisPosition );
    }

    public void CanLinkState(bool newState) 
    {
        _canLink = newState;
    }
    
    public bool TakeFirstLinked()
    {
        bool saveState = _isFirstLinkedNote;
        _isFirstLinkedNote = false;
        if (saveState == true)
        {
            return true;
        }
        return false;
    }
    public void ReleaseFirstLinked()
    {
        _isFirstLinkedNote = true;
        _inLine = true;
    }

    public bool IsInLine()
    {
        return _inLine;
    }
    public void ChangeInLineState(bool newState)
    {
        _inLine = newState;
    }

    public void PlayPauseGame(bool play)
    {
        if (play)
        {
            metronome.AudioSourceMusic.UnPause();
            return;
        }

        metronome.AudioSourceMusic.Pause();
    }
}
