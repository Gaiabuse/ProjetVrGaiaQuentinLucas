using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

public class NoteScript : MonoBehaviour
{
    [SerializeField] private float damages = 2f;
    [SerializeField] private float hitSpeed = 1f;
    [SerializeField] private int maxHit = 5;
    [SerializeField] protected float timeForPerfect = 0.2f;
    [SerializeField] protected float perfectMultiplier = 2f;

    [SerializeField] private float inputActivation = 0.5f;
    [SerializeField] protected GameObject perfectEffectPrefab;
    
    protected FightInputsActions inputs;
    
    private bool _canHit = true;
    protected float spawnTime;
    protected bool _inTrigger = false;
    
    protected InputDevice _leftHand;
    protected InputDevice _rightHand;

    protected virtual void Awake()
    {
        StartCoroutine(WaitForDamages());
        inputs = new FightInputsActions();
        inputs.Enable();
        inputs.Click.LeftTrigger.performed += OnLeftPressed;
        inputs.Click.RightTrigger.performed += OnRightPressed;
        _leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        _rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        spawnTime = Time.time;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inTrigger = false;
        }    
    }
    

    protected virtual void OnLeftPressed(InputAction.CallbackContext ctx)
    {
        if (!_inTrigger)return;
        
        if (Time.time - spawnTime < timeForPerfect)
        {
            FightManager.INSTANCE.AddAnxiety(- damages * perfectMultiplier);
            GameObject perfectGO = Instantiate(perfectEffectPrefab);
            perfectGO.transform.position = transform.position;
        }
        else
        {
            FightManager.INSTANCE.AddAnxiety(- damages);
        }
        
        _leftHand.SendHapticImpulse(0, 0.5f, 0.1f);
        
        Destroy(gameObject);
    }

    protected virtual void OnRightPressed(InputAction.CallbackContext ctx)
    {
        if (!_inTrigger)return;
        
        if (Time.time - spawnTime < timeForPerfect)
        {
            FightManager.INSTANCE.AddAnxiety(- damages * perfectMultiplier);
            GameObject perfectGO = Instantiate(perfectEffectPrefab);
            perfectGO.transform.position = transform.position;
        }
        else
        {
            FightManager.INSTANCE.AddAnxiety(- damages);
        }
        
        _rightHand.SendHapticImpulse(0, 0.5f, 0.1f);
        Destroy(gameObject);
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

    protected virtual void OnDestroy()
    {
        inputs.Click.LeftTrigger.performed -= OnLeftPressed;
        inputs.Click.RightTrigger.performed -= OnRightPressed;
        StopAllCoroutines();
    }
}
