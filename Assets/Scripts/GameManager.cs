
using System;
using com.gaïa.utils;
using Exploration;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    
    public static GameManager INSTANCE;
    private Vector3 _startPositionGame;
    public Coroutine DayLoop;

    public static Action ReturnToMainMenu;

    public enum GameState { Start, Playing, Paused}
    private GameState _state = GameState.Start;
    
    public bool ChoiceSelected = false;
    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        transform.ResetTransform();
    }
    
    public void SetState(GameState newState)
    {
        _state = newState;
    }

    public GameState GetState()
    {
        return _state;
    }
    void Start()
    {
        ChoiceSelected = false;
        PlayerManager.INSTANCE.SetCanMove(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
