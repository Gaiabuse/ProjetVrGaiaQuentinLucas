using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private float _anxiety = 0f;
    private bool _canLink = false;
    private bool _isFirstLinkedNote = true;
    
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
        StartFight(level); // à effacer, uniquement pour tests
    }

    #region Start / End
    
    public void StartFight(LevelData newLevel)
    {
        //SceneManager.LoadScene();    // Load la scene de combat (quand on aura toutes les scenes)
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

    void EndFight()
    {
        _anxiety = 0f;
        metronome.EndFight();
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
            Debug.Log("You loose"); // changer la plupart des trucs ici c'est uniquement pour le rendu du 13 a 16h c'est pas definitif
            EndFight();
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
                
                LinkedNotes linked = actualGO.GetComponent<LinkedNotes>(); // désolé Jacques j'ai honte mais pas le temps
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
                if (_canLink)
                {
                    return;
                }

                CanLink();

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

    public void CanLink()
    {
        _canLink = true;
    }
    public void CantLink()
    {
        _canLink = false;
    }
    
}
