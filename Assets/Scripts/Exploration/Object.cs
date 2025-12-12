using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Exploration
{
    public class Object : MonoBehaviour
    {
        [Header("Référence Data")]
        [SerializeField]private ObjectData objectData;
    
        [Header("Références UI")]
        [SerializeField] private GameObject panelDescription; 
        [SerializeField] private TextMeshProUGUI textDescription;

        private bool _startFonction = false;
        private bool _inHook = false;

        private void Start()
        {
            StartCoroutine(StartFunctionAfterDelay());
        }
        private void FixedUpdate()
        {
            if (_inHook && panelDescription.activeInHierarchy)//SecondCheck
            {
                panelDescription.SetActive(false);
            }
        }
        public void TakeObject(SelectEnterEventArgs args)
        {
            if (objectData == null || !_startFonction) return;
            
            AddObject();
            ActiveDescription();
        }
        private void ActiveDescription()
        {
            if (panelDescription == null || textDescription == null) return;
            textDescription.text = objectData.DescriptionObject;
            panelDescription.SetActive(!panelDescription.activeInHierarchy);
        }

        private void AddObject()
        {
            if (PlayerManager.INSTANCE.CheckObjectObtained(objectData)) return;
            PlayerManager.INSTANCE.AddObject(objectData);
            NotificationManager.INSTANCE.SetObjectNotification(objectData.ObjectName);
            StartCoroutine(NotificationManager.INSTANCE.ShowNotification());
        }

        public void SetInHook(bool inHook) => this._inHook = inHook;
       
        // start function with time delay for don't active at the start the function for Take object
        private IEnumerator StartFunctionAfterDelay()
        {
            _startFonction = false;
            yield return new WaitForSecondsRealtime(0.1f);
            _startFonction = true;
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