using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;
    [SerializeField] private float timeForPerfect = 0.2f;
    [SerializeField] private float perfectMultiplier = 2f;

    [SerializeField] private float inputActivation = 0.5f;
    
    private bool _canHit = true;
    private float spawnTime;
    protected bool _inTrigger = false;
    
    protected InputDevice _leftHand;
    protected InputDevice _rightHand;

    protected virtual void Awake()
    {
        StartCoroutine(WaitForDamages());
        _leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        _rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        spawnTime = Time.time;
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
            bool rightPressed = _rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueRight) && triggerValueRight > 0.1f;
            bool leftPressed  = _leftHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValueLeft) && triggerValueLeft > 0.1f;
            if (rightPressed || leftPressed) 
            {
                if (Time.time - spawnTime < timeForPerfect)
                {
                    FightManager.INSTANCE.AddAnxiety(- damages * perfectMultiplier);
                    Debug.Log("Perfect");
                }
                else
                {
                    FightManager.INSTANCE.AddAnxiety(- damages);
                }
                if (rightPressed)
                {
                    _rightHand.SendHapticImpulse(0, 0.5f, 0.1f);
                }
                else
                {
                    _leftHand.SendHapticImpulse(0, 0.5f, 0.1f);
                }
                Destroy(gameObject);
            }
            
            yield return null;
        }
    }

    IEnumerator WaitForDamages()
    {
        if (_canHit)
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
