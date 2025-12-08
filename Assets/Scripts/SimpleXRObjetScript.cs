using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class SimpleXRObjetScript : MonoBehaviour
{
    [Header("Référence Data")]
    public SuccessData successData;
    
    [Header("Références UI")]
    [SerializeField] private GameObject panelDescription; 
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private GameObject panelSuccess; 
    [SerializeField] private TextMeshProUGUI textSuccess;

    private bool startFonction = false;
    private bool inHook = false;
    public void ActiveDescriptionAndVerifSuccess(SelectEnterEventArgs args)
    {
        
        if (successData == null || !startFonction) return;
        if (!PlayerConditionManager.instance.CheckSuccessObtained(successData))
        {
            SuccessManager.INSTANCE.UnlockSuccess(successData);
            textSuccess.text = successData.descriptionSuccess;
            panelSuccess.SetActive(true);
            StartCoroutine(SuccessManager.INSTANCE.DisplayNotificationCo());
        }
        if (panelDescription != null && textDescription != null)
        {
            textDescription.text = successData.descriptionObjet;
            panelDescription.SetActive(!panelDescription.activeInHierarchy);
     
        }

        
    }

    public void SetInHook(bool inHook) => this.inHook = inHook;
    private void Start()
    {
        StartCoroutine(StartFunction());
    }

    private IEnumerator StartFunction()
    {
        startFonction = false;
        yield return new WaitForSecondsRealtime(0.1f);
        startFonction = true;
    }
    private void FixedUpdate()
    {
        if (inHook && panelDescription.activeInHierarchy)
        {
            panelDescription.SetActive(false);
        }
    }

    public void DesactiveDescriptionApresGrab(SelectExitEventArgs args)
    {
        if (panelDescription != null)
        {
            Debug.Log("inschallah");
            panelDescription.SetActive(false);
        }
    }
    
}