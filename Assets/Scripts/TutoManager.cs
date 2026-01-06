using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image illustration;

    [SerializeField] private float tweenDuration = 0.5f;
    
    [SerializeField] TutoData[] dataArray;
    
    private int _actualIndex = 0;
    private Vector3 _baseWindowSize;
    private bool _isInWindow = false;
    protected FightInputsActions inputs;



    public void StartTuto(LevelData tutoLevelData)
    {
        _actualIndex = 0;
        FightManager.INSTANCE.StartFight(tutoLevelData);
        ShowNextWindow();
    }
    
    private void Awake()
    {
        inputs = new FightInputsActions();
        inputs.Enable();
        
        inputs.Click.ClickButton.performed += OnClickButton;
        _baseWindowSize = window.transform.localScale;
    }
    void PausePlayGame(bool play)
    {
        Time.timeScale = play ? 1 : 0;
    }

    void ShowNextWindow()
    {
        PausePlayGame(false);
        FightManager.INSTANCE.PlayPauseGame(false);
        window.SetActive(true);
        _isInWindow = true;
        window.transform.DOScale(_baseWindowSize,tweenDuration).SetEase(Ease.Linear).SetUpdate(true);
        text.text = dataArray[_actualIndex].Text;
        illustration.sprite = dataArray[_actualIndex].Illustration;
        _actualIndex++;
    }

    private IEnumerator NextTutoWindow(float timeToWait)
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        ShowNextWindow();
    }

    bool IsEnd()
    {
        if (_actualIndex >= dataArray.Length) return true;
        return false;
    }

    void ContinueTuto()
    {
        PausePlayGame(true);
        FightManager.INSTANCE.PlayPauseGame(true);
        _isInWindow = false;
        window.transform.DOScale(Vector3.zero, tweenDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                window.SetActive(false);
            });
        if (IsEnd())
        {
            return;
        }
        StartCoroutine(NextTutoWindow(dataArray[_actualIndex - 1].TimeBeforeNextWindow));
    }
    
    public void OnClickButton(InputAction.CallbackContext ctx)
    {
        if (!_isInWindow) return;

        ContinueTuto();
    }

    private void OnDestroy()
    {
        inputs.Click.ClickButton.performed -= OnClickButton;
    }
}
