using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuGameObject;
    [SerializeField] private InputActionProperty keyForPause;


    private GameManager.GameState state;
    
    private void Start()
    {
        pauseMenuGameObject.SetActive(false);
    }
    
    void OnEnable()
    {
        keyForPause.action.performed += OnPauseButton;
        keyForPause.action.Enable();
    }

    void OnDisable()
    {
        keyForPause.action.performed -= OnPauseButton;
        keyForPause.action.Disable();
    }
    
    private void OnPauseButton(InputAction.CallbackContext ctx)
    {
        state= GameManager.INSTANCE.GetState();
        if(state == GameManager.GameState.Start)return;
        switch (state)
        {
            case GameManager.GameState.Playing:
                Paused();
                break;
            case GameManager.GameState.Paused:
                ResumeGame();
                break;
        }
    }
    
    public void Paused()
    {
        GameManager.INSTANCE.SetState(GameManager.GameState.Paused);

        PlayerManager.INSTANCE.SetCanMove(false);

        pauseMenuGameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {

        PlayerManager.INSTANCE.SetCanMove(true);

        pauseMenuGameObject.SetActive(false);

        GameManager.INSTANCE.SetState(GameManager.GameState.Playing);
        Time.timeScale = 1;
    }
    
    public void ReturnMenu()
    {
        GameManager.INSTANCE.SetState(GameManager.GameState.Start);
        pauseMenuGameObject.SetActive(false);
        GameManager.ReturnToMainMenu?.Invoke();
        PlayerManager.INSTANCE.SetCanMove(false);
        StopCoroutine(GameManager.INSTANCE.DayLoop);
        Time.timeScale = 1;
    }
}
