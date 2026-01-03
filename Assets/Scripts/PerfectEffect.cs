using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PerfectEffect : MonoBehaviour
{
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private float timeToDespawn = 1f;

    private Vector3 _baseScale;
    private void Awake()
    {
        _baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(_baseScale, tweenDuration).SetEase(Ease.OutBack);
        StartCoroutine(DespawnCoroutine());

    }

    IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSecondsRealtime(timeToDespawn);
        transform.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
