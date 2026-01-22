using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;
    // Current game state
    public GameState State;
    // Event for game state changes
    public event System.Action<GameState> OnGameStateChanged;

    void Awake()
    {
        // Ensure only one instance of GameManager exists
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.Initialisation);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Initialisation:
                // Handle initialisation logic
                break;
            case GameState.Menu:
                // Handle menu logic
                break;
            case GameState.Loading:
                // Handle loading logic
                break;
            case GameState.Playing:
                // Handle playing logic
                break;
            case GameState.Paused:
                // Handle paused logic
                break;
            case GameState.LevelCompleted:
                // Handle level completed logic
                break;
            case GameState.GameOver:
                // Handle game over logic
                break;
            case GameState.Credits:
                // Handle credits logic
                break;
            default:
                Debug.LogError("Unhandled game state: " + newState.ToString());
                break;
        }
        //Guard against null error when events not subscribed
        OnGameStateChanged?.Invoke(newState);
        Debug.Log("Game State updated to: " + newState.ToString());
    }
}

//Collection of possible game states. Note: Can be expanded or reduced as needed.
public enum GameState
{
    Initialisation,
    Menu,
    Loading,
    Playing,
    Paused,
    LevelCompleted,
    GameOver,
    Credits
} 
