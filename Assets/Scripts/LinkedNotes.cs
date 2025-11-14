using System;
using DG.Tweening;
using UnityEngine;

public class LinkedNotes : NoteScript // a clean mieux pour la beta
{
    [SerializeField] private int nextNoteMaxDistance = 6;
    [SerializeField] private float allowedDistance = 0.15f;
    [SerializeField] private float drawSpeed = 4f;
    [SerializeField] private float counterDamages = -2f;
    private Vector3 _nextNotePos;
    private Vector3Int _sheetMusicPosition;
    private LineRenderer _line;
    private float _progress;
    private bool _active;
    private bool _lineBroken = false;

    private const int LinkedNoteIndex = 2;

    public void ChangeSheetMusicPosition(Vector3Int newPosition)
    {
        _sheetMusicPosition = newPosition;
        CheckNextNote();
    }

    void Update()
    {
        if (!_active) return;

        if (_progress < 1f)
        {
            _progress += Time.deltaTime * drawSpeed;
            if (_progress > 1f) _progress = 1f;

            Vector3 pos = Vector3.Lerp(transform.position, _nextNotePos, _progress);
            _line.SetPosition(1, pos);
        }
    }


    void CheckNextNote()
    {
        Debug.Log(_sheetMusicPosition);

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

        while (divisionsCounted < nextNoteMaxDistance)
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
        DOTween.To(
            () => 0f,
            t => _line.SetPosition(1, Vector3.Lerp(transform.position, _nextNotePos, t)),
            1f,
            duration
        ).SetEase(Ease.Linear);
    }

    bool HandOnLine(Vector3 pos1, Vector3 pos2)
    {
        Collider[] collisions = Physics.OverlapSphere(pos1, 2f);
        float best = float.MaxValue;

        foreach (Collider c in collisions)
        {
            if (!c.CompareTag("Player")) continue;
            float distance = DistancePointToSegment(c.transform.position, pos1, pos2);
            if (distance < best) best = distance;
        }

        return best <= allowedDistance;
    }
    bool CheckHandOnLine(Vector3 handPos)
    {
        if (_lineBroken) return false;

        float distance = DistancePointToSegment(handPos, transform.position, _nextNotePos);
        if (distance > allowedDistance)
        {
            _lineBroken = true;
            return false;
        }
        return true;
    }


    float DistancePointToSegment(Vector3 playerPos, Vector3 startPos, Vector3 endPos)
    {
        Vector3 startToPlayer = playerPos - startPos;
        Vector3 startToEnd = endPos - startPos;

        float projection = Vector3.Dot(startToPlayer, startToEnd) / startToEnd.sqrMagnitude;
        projection = Mathf.Clamp01(projection);

        Vector3 closestPoint = startPos + startToEnd * projection;

        return Vector3.Distance(playerPos, closestPoint);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_lineBroken) return; 

        if (other.CompareTag("Player"))
        {
            FightManager.INSTANCE.AddAnxiety(counterDamages);
            Destroy(gameObject);
        }
    }

}
