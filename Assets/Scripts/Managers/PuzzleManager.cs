using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public IPuzzle.PuzzleState State { get; set; }

    public event System.Action<IPuzzle.PuzzleState> OnPuzzleStateChanged;
    private bool _isSubscribedToGameManager;
    public bool HasStateInitialized { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

    private void OnEnable()
    {
        Debug.Log("PuzzleManager OnEnable -> attempting GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void Start()
    {
        Debug.Log("PuzzleManager Start -> retrying GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void OnDisable()
    {
        Debug.Log($"PuzzleManager OnDisable -> subscribed={_isSubscribedToGameManager}");
        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        _isSubscribedToGameManager = false;
    }

    public void UpdatePuzzleState(IPuzzle.PuzzleState newPuzzleState)
    {
        State = newPuzzleState;

        switch (newPuzzleState)
        {
            case IPuzzle.PuzzleState.Solved:
                // Handle solved-specific logic here if needed
                break;
            case IPuzzle.PuzzleState.Unsolved:
                // Handle unsolved-specific logic here if needed
                break;
            case IPuzzle.PuzzleState.InProgress:
                // Handle in-progress-specific logic here if needed
                break;
        }
        HasStateInitialized = true;
        OnPuzzleStateChanged?.Invoke(State);
        Debug.Log($"Puzzle state updated to: {State}");
    }    

    public void HandleGameStateChanged(GameState newState)
    {
        Debug.Log($"PuzzleManager received GameState: {newState}");
        switch (newState)
        {
            case GameState.Initialisation:
                // Handle any puzzle-specific logic for initialisation if needed
                break;
            case GameState.Menu:
                // Handle any puzzle-specific logic for menu if needed
                break;
            case GameState.Loading:
                // Handle any puzzle-specific logic for loading if needed
                break;
            case GameState.Playing:
                // Handle any puzzle-specific logic for playing if needed
                break;
            case GameState.Paused:
                // Handle any puzzle-specific logic for paused if needed
                break;
            case GameState.LevelCompleted:
                // Handle any puzzle-specific logic for level completed if needed
                break;
            case GameState.GameOver:
                // Handle any puzzle-specific logic for game over if needed
                break;
            case GameState.Credits:
                // Handle any puzzle-specific logic for credits if needed
                break;
            default:
                break;
        }
    }

    private void TrySubscribeToGameManager()
    {
        if (_isSubscribedToGameManager)
        {
            Debug.Log("PuzzleManager subscription skipped (already subscribed).");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.Log("PuzzleManager subscription deferred (GameManager.Instance is null).");
            return;
        }

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _isSubscribedToGameManager = true;
        Debug.Log("PuzzleManager subscribed to GameManager.OnGameStateChanged.");

        if (GameManager.Instance.HasStateInitialized)
        {
            Debug.Log($"PuzzleManager late-syncing to current GameState: {GameManager.Instance.State}");
            HandleGameStateChanged(GameManager.Instance.State);
        }
    }

    public void RegisterPuzzle(IPuzzle puzzle)
    {
        // TODO: Add logic to track registered puzzles
    }

    public void ResetAllPuzzles()
    {
        // TODO: Add logic to reset all registered puzzles to their initial state
    }
}
