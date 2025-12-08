using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class LinkedNotes : NoteScript
{
    [SerializeField] private int nextNoteMaxDistance = 6;
    [SerializeField] private float counterDamages = -2f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject particles;
    [SerializeField] private float maxLineTime = 1.5f;
    [SerializeField] private Link link;
    
    private Vector3 _nextNotePos;
    private Vector3Int _sheetMusicPosition;
    private bool _isFirstLinked = false;
    

    private const int LinkedNoteIndex = 2;

    public void ChangeSheetMusicPosition(Vector3Int newPosition)
    {
        _sheetMusicPosition = newPosition;
        int divisionToNextNote = CountDivisionsToNextNote();
        if (divisionToNextNote <= nextNoteMaxDistance)
        {
            FightManager.INSTANCE.CanLinkState(true);
            if (FightManager.INSTANCE.TakeFirstLinked())
            {
                _isFirstLinked = true;
            }
            CheckNextNote();
        }
        else
        {
            FightManager.INSTANCE.CanLinkState(false);
            FightManager.INSTANCE.ReleaseFirstLinked();
            link.Destroy();
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
    int CountDivisionsToNextNote()
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
        int divisionsToNextNote = CountDivisionsToNextNote();
        float timePerDivision = 60f / FightManager.INSTANCE.GetLevel().bpm / FightManager.INSTANCE.GetLevel().beat;
        float duration = timePerDivision * divisionsToNextNote;
        link.Move(_nextNotePos, duration);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (FightManager.INSTANCE.IsInLine())
        {
            base.OnTriggerEnter(other);
        }
    }

    protected override IEnumerator PlayerInTrigger()
    {
        while (_inTrigger)
        {
            bool rightPressed = rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueRight) && triggerValueRight > 0.1f;
            bool leftPressed  = leftHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueLeft) && triggerValueLeft > 0.1f;
            if (rightPressed || leftPressed)
            {
                FightManager.INSTANCE.AddAnxiety(counterDamages);
                if (rightPressed)
                {
                    rightHand.SendHapticImpulse(0, 0.5f, maxLineTime);
                }
                else
                {
                    leftHand.SendHapticImpulse(0, 0.5f, maxLineTime);
                }
                StartCoroutine(WaitForDestroy());
            }
            else if (!_isFirstLinked)
            {
                FightManager.INSTANCE.InLineState(false);
                yield break;
            }

            yield return null;
            
        }
    }
    
    

    IEnumerator WaitForDestroy()
    {
        FightManager.INSTANCE.AddAnxiety(counterDamages);
        Destroy(meshRenderer);
        Destroy(particles);
        yield return new WaitForSeconds(maxLineTime);
        Destroy(gameObject);
    }
    

}
