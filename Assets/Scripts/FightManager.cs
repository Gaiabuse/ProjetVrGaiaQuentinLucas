using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class FightManager : MonoBehaviour
{
    public static FightManager INSTANCE;
    
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
    
    
    private Vector2[,,] spawnPositions;
    private int[,,] sheetMusic;
    public static Action<bool> FightEnded;
    private float anxiety = 0f;
    private bool canLink = false;
    private bool inLine = true;
    private bool isFirstLinkedNote = true;
    private InputDevice leftHand;
    private InputDevice rightHand;
    
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
        
        spawnPositions = level.spawnPositions;
        sheetMusic = level.sheetMusic;
        metronome.ChangeValues(level.audioClip, level.bpm, level.beat, level.division);
    }
    

    IEnumerator WaitForStartMusic()
    {
        yield return new WaitForSecondsRealtime(3f);
        metronome.audioSource.Play();
    }

    public void EndFight(bool win)
    {
        anxiety = 0f;
        PlayerManager.INSTANCE.AddLevelData(level,win);
        metronome.EndFight();
        FightEnded.Invoke(win);
    }
    
    #endregion
    
    
    public void AddAnxiety(float value)
    {
        anxiety += value;
        slider.value = anxiety;
        CheckLoose();
    }

	public LevelData GetLevel()
	{
		return level;
	}


    void CheckLoose()
    {
        if (anxiety >= maxAnxiety)
        {
            EndFight(false);
        }
    }
    
    public void NoteSpawn(int actualMeasure, int actualBeat, int actualDivision)
    {
        int actualNote = sheetMusic[actualMeasure, actualBeat, actualDivision];
        
        if (actualNote > notesPrefabs.Length)
        {
            return;
        }
        
        Vector2 actualPos = spawnPositions[actualMeasure, actualBeat, actualDivision];

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
        
        int actualPreviewNote = sheetMusic[actualMeasure, actualBeat, actualDivision];
        
        if (actualPreviewNote > previewNotesPrefabs.Length)
        {
            return;
        }
        
        Vector2 actualPos = spawnPositions[actualMeasure, actualBeat, actualDivision];
        
        if (actualPreviewNote != 0)
        {
            if (actualPreviewNote == 2 )
            {
                if (canLink)
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
        return level.sheetMusic[measure, beat, division];
    }
    public Vector3 GetPos(int measure, int beat, int division)
    {
        return new Vector3(level.spawnPositions[measure, beat, division].x / spawnPosDivider,level.spawnPositions[measure, beat, division].y / spawnPosDivider, zAxisPosition );
    }

    public void CanLinkState(bool newState) 
    {
        canLink = newState;
    }
    

    public bool TakeFirstLinked()
    {
        bool saveState = isFirstLinkedNote;
        isFirstLinkedNote = false;
        if (saveState == true)
        {
            return true;
        }
        return false;
    }
    public void ReleaseFirstLinked()
    {
        isFirstLinkedNote = true;
        inLine = true;
    }

    public bool IsInLine()
    {
        return inLine;
    }
    public void InLineState(bool newState)
    {
        inLine = newState;
    }
    
}
