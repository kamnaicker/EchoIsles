using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public PuzzleState puzzleState;
    public event System.Action<PuzzleState> OnPuzzleStateChanged;

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

    public void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    public void UpdatePuzzleState(PuzzleState newPuzzleState)
    {
        puzzleState = newPuzzleState;

        switch (newPuzzleState)
        {
            case PuzzleState.Solved:
                // Handle solved-specific logic here if needed
                break;
            case PuzzleState.Unsolved:
                // Handle unsolved-specific logic here if needed
                break;
        }

        OnPuzzleStateChanged?.Invoke(puzzleState);
        Debug.Log($"Puzzle state updated to: {puzzleState}");
    }    

    public void HandleGameStateChanged(GameState newState)
    {
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
}

public enum PuzzleState
{
    Solved,
    Unsolved,

}
