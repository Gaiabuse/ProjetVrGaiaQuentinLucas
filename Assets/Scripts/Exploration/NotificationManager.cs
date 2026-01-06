using System.Collections;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Exploration
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager INSTANCE;
        
        [Header("UI Object Notification")]
        [SerializeField] private GameObject notificationDisplay;
        [SerializeField] private TextMeshProUGUI takeObjectNotification;
        [SerializeField] private float displayDuration = 3.0f;

        private void Awake()
        {
            if (INSTANCE == null)
            {
                INSTANCE = this;
            }
            else
            {
                Destroy(gameObject);
                // ptet un petit return ici pour éviter le code appellé dessous
            }

            if (notificationDisplay != null)
            {
                notificationDisplay.SetActive(false);
            }
        }
        
        public void SetObjectNotification(string message)
        {
            if (notificationDisplay != null && takeObjectNotification != null)
            {
                takeObjectNotification.text = message;
                // assez risqué le stopAllCoroutines, tu pouvais vraiment pas
                // récupérer une potentielle ref vers la showNotification pour la stop que elle?
                // c'est surtout si ton script évolue, et qu'il se retrouve à avoir d'autres coroutines,
                // tu risques de pas comprendre pourquoi ca pete
                // je vois pas mal de stopAllCoroutines dans le projet, caca incoming dans le futur avec ca, je préviens
                StopAllCoroutines(); 
                StartCoroutine(ShowNotification());
            }
        }
        public IEnumerator ShowNotification()
        {
            notificationDisplay.SetActive(true); 
            yield return new WaitForSeconds(displayDuration);
            notificationDisplay.SetActive(false); 
        }
    }
}