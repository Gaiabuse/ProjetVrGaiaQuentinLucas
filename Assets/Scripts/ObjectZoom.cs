using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjectZoom : MonoBehaviour
{
    [SerializeField] private XROrigin origin;
    [SerializeField] private float smoothTime = 1.5f;
    [SerializeField]private float zoomValue = 30f;
    private bool objectGrabbed = false;
    private bool zoom= false;
    private float velocity = 0f;
    private float startFieldOfView;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (zoom && objectGrabbed)
        {
            Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, zoomValue, ref velocity, smoothTime );
            if(Camera.main.fieldOfView <= zoomValue)zoom = false;
        }else if (zoom && !objectGrabbed)
        {
            Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, startFieldOfView, ref velocity, smoothTime );
            if(Camera.main.fieldOfView <= startFieldOfView)zoom = false;
        }
    }

    public void GrabObject(SelectEnterEventArgs args)
    {
       
        if (args.interactorObject is not XRSocketInteractor)
        {
            startFieldOfView = Camera.main.fieldOfView;
            objectGrabbed = true;
            zoom = true;
            Camera.main.transform.LookAt(transform.position);
        }
       
    }
  
    public void UngrabObject(SelectExitEventArgs args)
    {
        if (args.interactorObject is not XRSocketInteractor)
        {
            objectGrabbed  = false;
            zoom = true;
        }
      
    }
    
}
