using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;
    // Current game state
    public GameState State;
    public bool HasStateInitialized { get; private set; }
    // Event for game state changes
    public event System.Action<GameState> OnGameStateChanged;

    void Awake()
    {
        // Ensure only one instance of GameManager exists
        if(Instance == null)
        {
            Instance = this;
            //Persist GameManager across all scenes
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    void Start()
    {
        Debug.Log("GameManager boot sequence starting.");
        UpdateGameState(GameState.Initialisation);
        SyncStateToCurrentScene();
    }

    public void UpdateGameState(GameState newState)
    {
        Debug.Log($"GameManager transitioning to {newState}.");
        State = newState;

        switch (newState)
        {
            case GameState.Initialisation:
                Debug.Log("Game Initialising...");
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
        HasStateInitialized = true;
        //Guard against null error when events not subscribed
        Debug.Log($"GameManager broadcasting state change: {newState}");
        OnGameStateChanged?.Invoke(newState);
        Debug.Log("Game State updated to: " + newState.ToString());
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SyncStateToCurrentScene();
    }

    private void SyncStateToCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
            return;

        bool isMenuScene = activeScene.name == "Main Menu";
        GameState targetState = isMenuScene ? GameState.Menu : GameState.Playing;

        if (State == targetState)
            return;

        UpdateGameState(targetState);
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
