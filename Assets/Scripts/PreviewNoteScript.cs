using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PreviewNoteScript : MonoBehaviour
{
    [SerializeField] private float targetZ = 0.3f;
    
    private LevelData _level;
    private Vector3 _targetPos;

    private void Awake()
    {
        if (FightManager.INSTANCE != null)
        {
            _level = FightManager.INSTANCE.GetLevel();
            StartCoroutine(WaitForFrame());
        }
    }

    IEnumerator WaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        _targetPos = new Vector3(transform.position.x, transform.position.y, targetZ);
        StartMovement();
    }

    void StartMovement()
    {
        transform.DOMove(_targetPos, 60f /_level.bpm * _level.beat).OnComplete(() =>
        {
            Destroy(gameObject);
        }).SetEase(Ease.Linear);
    }
}
