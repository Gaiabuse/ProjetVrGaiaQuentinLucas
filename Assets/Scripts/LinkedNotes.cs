using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LinkedNotes : NoteScript // a clean mieux pour la beta
{
    [SerializeField] private int nextNoteMaxDistance = 6;
    [SerializeField] private float counterDamages = -2f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject particles;
    [SerializeField] private float maxLineTime = 1.5f;
    private Vector3 _nextNotePos;
    private Vector3Int _sheetMusicPosition;
    private LineRenderer _line;
    private float _progress;
    private bool _active;
    

    private const int LinkedNoteIndex = 2;

    public void ChangeSheetMusicPosition(Vector3Int newPosition)
    {
        _sheetMusicPosition = newPosition;
        int divisionToNextNote = CountDivisionsToNextNote();
        Debug.Log(divisionToNextNote);
        if (divisionToNextNote <= nextNoteMaxDistance)
        {
            FightManager.INSTANCE.CanLinkState(true);
            CheckNextNote();
        }
        else
        {
            FightManager.INSTANCE.CanLinkState(false);
        }
    }

    void CheckNextNote()
    {
        
        int maxMeasure = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(0);
        int maxBeat = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(1);
        int maxDivision = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(2);
        int remainingDistance = nextNoteMaxDistance;
        int measure = _sheetMusicPosition.x;
        int beat = _sheetMusicPosition.y;
        int division = _sheetMusicPosition.z;

        while (remainingDistance > 0)
        {
            division++;
            if (division >= maxDivision)
            {
                division = 0;
                beat++;
                if (beat >= maxBeat)
                {
                    beat = 0;
                    measure++;
                    if (measure >= maxMeasure)
                        break;
                }
            }

            int noteType = FightManager.INSTANCE.GetNote(measure, beat, division);
            if (noteType == LinkedNoteIndex)
            {
                _nextNotePos = FightManager.INSTANCE.GetPos(measure, beat, division);
                LinkNote();
                break;
            }

            remainingDistance--;
        }
    }
    int CountDivisionsToNextNote() // c'est quasiment pareil que la fonction d'avant . a changer
    {
        int maxMeasure = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(0);
        int maxBeat = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(1);
        int maxDivision = FightManager.INSTANCE.GetLevel().sheetMusic.GetLength(2);
        int measure = _sheetMusicPosition.x;
        int beat = _sheetMusicPosition.y;
        int division = _sheetMusicPosition.z;
        int divisionsCounted = 0;

        while (divisionsCounted < nextNoteMaxDistance + 1)
        {
            division++;
            if (division >= maxDivision)
            {
                division = 0;
                beat++;
                if (beat >= maxBeat)
                {
                    beat = 0;
                    measure++;
                    if (measure >= maxMeasure)
                        break;
                }
            }

            divisionsCounted++;

            int noteType = FightManager.INSTANCE.GetNote(measure, beat, division);
            if (noteType == LinkedNoteIndex)
            {
                return divisionsCounted;
            }
        }
        return divisionsCounted;
    }




    void LinkNote()
    {
        _line = gameObject.AddComponent<LineRenderer>();
        _line.positionCount = 2;
        _line.startWidth = 0.04f;
        _line.endWidth = 0.04f;
        _line.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        _line.startColor = Color.yellow;
        _line.endColor = Color.yellow;
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, transform.position);
        _progress = 0f;
        _active = true;
        int divisionsToNextNote = CountDivisionsToNextNote();
        float timePerDivision = 60f / FightManager.INSTANCE.GetLevel().bpm / FightManager.INSTANCE.GetLevel().beat;
        float duration = timePerDivision * divisionsToNextNote;
        DOTween.To(() => 0f, t => _line.SetPosition(1, 
            Vector3.Lerp(transform.position, _nextNotePos, t)), 1f, duration).SetEase(Ease.Linear);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            FightManager.INSTANCE.AddAnxiety(counterDamages);
            Destroy(meshRenderer);
            Destroy(particles);
            StartCoroutine(WaitForDestroy());
        }
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(maxLineTime);
        Destroy(gameObject);
        
    }
    

}
