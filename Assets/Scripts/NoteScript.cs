using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;

    [SerializeField] private float inputActivation = 0.5f;
    
    private bool canHit = true;
    protected bool _inTrigger = false;
    
    protected InputDevice leftHand;
    protected InputDevice rightHand;

    private void Awake()
    {
        StartCoroutine(WaitForDamages());
        leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inTrigger = true;
            StartCoroutine(PlayerInTrigger());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _inTrigger = false;
    }

    protected virtual IEnumerator PlayerInTrigger()
    {
        
        while (_inTrigger)
        {
            bool rightPressed = rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueRight) && triggerValueRight > 0.1f;
            bool leftPressed  = leftHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueLeft) && triggerValueLeft > 0.1f;
            if (rightPressed || leftPressed) 
            {
                FightManager.INSTANCE.AddAnxiety(- damages);
                if (rightPressed)
                {
                    rightHand.SendHapticImpulse(0, 0.5f, 0.1f);
                }
                else
                {
                    leftHand.SendHapticImpulse(0, 0.5f, 0.1f);
                }
                Destroy(gameObject);
            }
            
            yield return null;
        }
    }

    IEnumerator WaitForDamages()
    {
        if (canHit)
        {
            for (int i = 0; i < maxHit; i++)
            {
                yield return new WaitForSeconds(hitSpeed);
                FightManager.INSTANCE.AddAnxiety(damages);
            }
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
