using System;
using DG.Tweening;
using UnityEngine;

public class Link : MonoBehaviour
{
    private Tween _movementTween;
    public void Move(Vector3 targetPosition, float timeToComplete)
    {
        transform.LookAt(targetPosition);
        transform.rotation = new Quaternion(transform.rotation.x , transform.rotation.y ,
            transform.rotation.z , transform.rotation.w);
        _movementTween = transform.DOMove(targetPosition,timeToComplete).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    private void OnDestroy()
    {
        DOTween.Kill(_movementTween);
    }
}