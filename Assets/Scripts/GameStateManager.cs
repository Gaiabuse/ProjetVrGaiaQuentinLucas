using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsNoMove;
    [SerializeField] private GameObject UIStart;
    [SerializeField] private GameObject UIPause;
    [SerializeField] private Vector3 positionForStartGame;
    [SerializeField] private Transform player;
    [SerializeField] private InputActionProperty keyForPause;
    private Vector3 startPositionGame;
    private Game game;

    private Coroutine dayLoop;
    public static Action ReturnToMenu;
    public enum GameState { Start, Playing, Paused, ResumeGame}

    class Game
    {
        public GameState state = GameState.Start;
        public void SetState(GameState newState)
        {
            state = newState;
            Debug.Log("Nouvel état : " + state);
        }
    }

    void Start()
    {
        game = new Game();
        game.SetState(GameState.Start);

        UIStart.SetActive(true);
        UIPause.SetActive(false);

        MoveOnStart();
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
        if (game.state == GameState.Playing)
            Paused();
        else if (game.state == GameState.Paused)
            ResumeGame();
    }

    void MoveOnStart()
    {
        foreach (var obj in objectsNoMove)
            obj.SetActive(false);
    }

    public void Play()
    {
        game.SetState(GameState.Playing);

        UIStart.SetActive(false);

        foreach (var obj in objectsNoMove)
            obj.SetActive(true);

        dayLoop = StartCoroutine(DayManager.instance.DayLoop());
        startPositionGame = player.position;
        player.position = positionForStartGame;
    }

    public void Paused()
    {
        Debug.Log("en pause");
        game.SetState(GameState.Paused);

        foreach (var obj in objectsNoMove)
            obj.SetActive(false);

        UIPause.SetActive(true);
    }

    public void ResumeGame()
    {
        game.SetState(GameState.ResumeGame);

        foreach (var obj in objectsNoMove)
            obj.SetActive(true);

        UIPause.SetActive(false);

        game.SetState(GameState.Playing);
    }

    public void ReturnMenu()
    {
        game.SetState(GameState.Start);
        UIStart.SetActive(true);
        UIPause.SetActive(false);

        MoveOnStart();
        ReturnToMenu.Invoke();
        StopCoroutine(dayLoop);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
