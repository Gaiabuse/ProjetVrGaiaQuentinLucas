using System;
using DG.Tweening;
using UnityEngine;

public class Link : MonoBehaviour
{
    public void Move(Vector3 targetPosition, float timeToComplete)
    {
        transform.LookAt(targetPosition);
        transform.rotation = new Quaternion(transform.rotation.x , transform.rotation.y ,
            transform.rotation.z , transform.rotation.w);
        transform.DOMove(targetPosition,timeToComplete).SetEase(Ease.Linear).OnComplete(()=> Destroy());
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
    
}