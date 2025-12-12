using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Exploration
{
    public class SimpleXRObjetScript : MonoBehaviour
    {
        [Header("Référence Data")]
        public SuccessData successData;
    
        [Header("Références UI")]
        [SerializeField] private GameObject panelDescription; 
        [SerializeField] private TextMeshProUGUI textDescription;

        private bool startFonction = false;
        private bool inHook = false;
        public void ActiveDescriptionAndVerifSuccess(SelectEnterEventArgs args)
        {
        
            if (successData == null || !startFonction) return;
            if (!PlayerManager.INSTANCE.CheckSuccessObtained(successData))
            {
                SuccessUIManager.INSTANCE.UnlockSuccess(successData);
                SuccessUIManager.INSTANCE.ShowNotification(successData.descriptionSuccess);
                StartCoroutine(SuccessUIManager.INSTANCE.DisplayNotificationCo());
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
            StartCoroutine(StartFunctionAfterDelay());
        }

        private IEnumerator StartFunctionAfterDelay()// start function with time delay for don't active at the start the function For Description
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

        public void DisableDescriptionAfterGrab(SelectExitEventArgs args)
        {
            if (panelDescription != null)
            {
                panelDescription.SetActive(false);
            }
        }
    
    }
}