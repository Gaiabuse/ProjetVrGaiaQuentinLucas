using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Exploration
{
    // de manièere générale, je te déconseille les noms de classes/variables qui sont déja des noms utilisés par ton langage
    // tu risques par exemple dans ton code d'avoir object, et Object, et douter de si tu utilises le type object ou ta classe
    // ( en plus quand on sait les risques d'utiliser object ( celui du c# ), ca fait toujours un peu peur de lire Object )
    // Dans ton cas, j'aurais surement nommé Object Item, ou Collectible par exemple
    public class Object : MonoBehaviour
    {
        // j'titille, mais si tu mets l'accent, ca devient du francais, et c'est en anglais autour
        [Header("Réference Data")]
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
            // évite au maxxxxiiiiiiimmmmuuuuum les checks dans l'update que tu peux faire de manière evenementielle ( quand inHook change, par exemple )
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

        // j'avoue que je suis pas sur de comprendre ce que fait la logique du hook/Inhook, c'est quand tu grabs un item?
        public void SetInHook(bool inHook) => this._inHook = inHook;
       
        // start function with time delay for don't active at the start the function for Take object
        private IEnumerator StartFunctionAfterDelay()
        {
            _startFonction = false;
            yield return new WaitForSecondsRealtime(0.1f); 
            // est ce que tu voulais juste délayer d'une frame ? 
            // ma narine me dit que soit tu pourrais, worst case, faire un yield return null, ou, plus propre,
            // si c'est pour que ca s'appelle apres les autres starts, simplement
            // déplacer dans script execution order ton script pour qu'il s'execute apres les autres
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