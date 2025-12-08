using UnityEngine;
using TMPro;
using System.Collections;

public class SuccessManager : MonoBehaviour
{
    public static SuccessManager INSTANCE;

    [Header("UI de Notification")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;
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

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }

    public void UnlockSuccess(SuccessData data)
    {
        PlayerConditionManager.instance.AddSuccess(data);
        string message = $" NOUVEAU SUCCÈS : {data.descriptionSuccess} !";
        Debug.Log(message);
    }

    private void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            StopAllCoroutines(); 
            StartCoroutine(DisplayNotificationCo());
        }
    }
    

    public IEnumerator DisplayNotificationCo()
    {
        notificationPanel.SetActive(true); 
        yield return new WaitForSeconds(displayDuration);
        notificationPanel.SetActive(false); 
    }
}