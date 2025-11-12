using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    public static FightManager INSTANCE;
    public float damages = 5f;
    
    [SerializeField] private float maxAnxiety = 100f;
    [SerializeField] private MetronomeManager metronome;
    [SerializeField] private GameObject[] notesPrefabs;
    [SerializeField] private GameObject[] previewNotesPrefabs;
    [SerializeField] private LevelData level;
    [SerializeField] private float spawnPosDivider = 100f;
    [SerializeField] private float zAxisPosition = 0f;

    private Vector2[,,] spawnPositions;
    private int[,,] sheetMusic;

    private float _anxiety = 0f;
    
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
        //StartFight(level); // à effacer, uniquement pour tests
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
        Debug.Log("startmusic");
        metronome.audioSource.Play();
    }

    void EndFight()
    {
        _anxiety = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // changer ça aussi a terme
    }
    
    #endregion
    
    
    public void AddAnxiety()
    {
        _anxiety += damages;
        Debug.Log(("anxiety + "));
        CheckLoose();
    }
    // loose aussi anxiety
    
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
            GameObject actualGO = Instantiate(previewNotesPrefabs[actualPreviewNote - 1]);
            actualGO.transform.position = new Vector3(actualPos.x / spawnPosDivider, actualPos.y / spawnPosDivider, zAxisPosition);
        }
    }
    
}
