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