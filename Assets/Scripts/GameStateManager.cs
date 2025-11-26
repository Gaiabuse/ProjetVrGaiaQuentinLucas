using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver
    }
    
    class Game
    {
        public GameState state = GameState.Start;

        public void SetState(GameState newState)
        {
            state = newState;
            Debug.Log("Nouvel état : " + state);
        }
    }

    private Game game;

    void Start()
    {
        game = new Game();
        
        game.SetState(GameState.Start);
    }

    void Update()
    {
        
    }

    void MoveOnStart()
    {
        
    }
}