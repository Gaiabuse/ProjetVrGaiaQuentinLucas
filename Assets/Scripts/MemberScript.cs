using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MemberScript : MonoBehaviour
{
    [SerializeField] private PlushManager plushManagerReference;
    [SerializeField] private XRGrabInteractable cube;
    [SerializeField] private int memberId;
    [SerializeField] private bool isRight;
    private Vector3 _initialPosition;
    private float _maxDistance;
    

    private void Start()
    {
        _initialPosition = transform.position;
        _maxDistance = plushManagerReference.distanceBeforeBreak;
    }

    private void OnEnable()
    {
        cube.selectExited.AddListener(ResetPosition); 
        cube.selectEntered.AddListener(StartGrab);
    }

    private void StartGrab(SelectEnterEventArgs arg0)
    {
        StartCoroutine(PositionUpdateWhenGrabbed());
    }

    private IEnumerator PositionUpdateWhenGrabbed()
    {
        while (cube.isSelected)
        {
            float actualDistance = Vector3.Distance(transform.position, _initialPosition);
            Debug.Log(actualDistance + " distance actuelle");
            if ( actualDistance > _maxDistance)
            {
                plushManagerReference.Reset(memberId, isRight);
                Debug.Log("caca");
                break;
            }
            plushManagerReference.ChangeScale(memberId, actualDistance,isRight);
            plushManagerReference.Rotation(memberId, transform);
            yield return null;
        }
    }

    private void ResetPosition(SelectExitEventArgs arg0)
    {
        transform.position = _initialPosition;
        plushManagerReference.Reset(memberId, isRight);
    }
}
