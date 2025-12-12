using UnityEngine;
using TMPro;
using System.Collections;

public class SuccessUIManager : MonoBehaviour
{
    public static SuccessUIManager INSTANCE;

    [Header("UI Success")]
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
        PlayerManager.INSTANCE.AddSuccess(data);
        string message = $" NOUVEAU SUCCÈS : {data.descriptionSuccess} !";
        Debug.Log(message);
    }

    public void ShowNotification(string message)
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