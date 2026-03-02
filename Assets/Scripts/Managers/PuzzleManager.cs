using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    public IPuzzle.PuzzleState State { get; private set; }

    public event System.Action<IPuzzle.PuzzleState> OnPuzzleStateChanged;

    public event System.Action<string,string,IPuzzle.PuzzleState> OnPuzzleStateChangedDetails;

    private readonly Dictionary<string, IPuzzle> _registeredPuzzles = new Dictionary<string, IPuzzle>();

    private readonly Dictionary<string, IPuzzle.PuzzleState> _puzzleStates = new Dictionary<string, IPuzzle.PuzzleState>();

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

    public void UpdatePuzzleState(string sceneName, string puzzleId, IPuzzle.PuzzleState newPuzzleState)
    {
        if(string.IsNullOrWhiteSpace(sceneName) || string.IsNullOrWhiteSpace(puzzleId))
            return;

        string key = BuildPuzzleKey(sceneName, puzzleId);
        _puzzleStates[key] = newPuzzleState;

        if(SaveManager.Instance != null)
            SaveManager.Instance.SetPuzzleState(sceneName, puzzleId, newPuzzleState);

        State = newPuzzleState;
        HasStateInitialized = true;

        OnPuzzleStateChanged?.Invoke(State);
        OnPuzzleStateChangedDetails?.Invoke(sceneName, puzzleId, newPuzzleState);

        Debug.Log($"Puzzle '{puzzleId}' in scene '{sceneName}' updated to: {newPuzzleState}");
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

    private string BuildPuzzleKey(string sceneName, string puzzleId)
    {
        return $"{sceneName}::{puzzleId}";
    }

    private string GetActiveSceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.IsValid() ? scene.name : string.Empty;
    }

    private bool TryGetPuzzleIdentity(IPuzzle puzzle, out string sceneName, out string puzzleId)
    {
        sceneName = string.Empty;
        puzzleId = string.Empty;

        if (puzzle == null)
            return false;

        puzzleId = puzzle.PuzzleId;

        if (string.IsNullOrWhiteSpace(puzzleId))
            return false;

        if (puzzle is Component component)
            sceneName = component.gameObject.scene.name;
        else
            sceneName = GetActiveSceneName();

        return !string.IsNullOrWhiteSpace(sceneName);
    }


    public void RegisterPuzzle(IPuzzle puzzle)
    {
        if(!TryGetPuzzleIdentity(puzzle, out string sceneName, out string puzzleId))
        {
            Debug.LogWarning("Failed to register puzzle: unable to determine valid scene name and puzzle ID.");
            return;
        }

        string key = BuildPuzzleKey(sceneName, puzzleId);

        _registeredPuzzles[key] = puzzle;

        if (SaveManager.Instance != null &&
        SaveManager.Instance.TryGetPuzzleState(sceneName, puzzleId, out IPuzzle.PuzzleState savedState))
        {
            _puzzleStates[key] = savedState;
        }
        else
        {
            _puzzleStates[key] = puzzle.State;
        }

        Debug.Log($"Registered puzzle '{puzzleId}' in scene '{sceneName}' with initial state: {_puzzleStates[key]}");
    }

    public void UnregisterPuzzle(IPuzzle puzzle)
    {
        if(!TryGetPuzzleIdentity(puzzle, out string sceneName, out string puzzleId))
        {
            Debug.LogWarning("Failed to unregister puzzle: unable to determine valid scene name and puzzle ID.");
            return;
        }

        string key = BuildPuzzleKey(sceneName, puzzleId);
        _registeredPuzzles.Remove(key);

        Debug.Log($"Unregistered puzzle '{puzzleId}' in scene '{sceneName}'.");
    }

    public bool TryGetPuzzleState(string sceneName, string puzzleId, out IPuzzle.PuzzleState state)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || string.IsNullOrWhiteSpace(puzzleId))
        {
            state = IPuzzle.PuzzleState.Unsolved;
            return false;
        }

        string key = BuildPuzzleKey(sceneName, puzzleId);

        if (_puzzleStates.TryGetValue(key, out state))
            return true;

        if (SaveManager.Instance != null && SaveManager.Instance.TryGetPuzzleState(sceneName, puzzleId, out state))
        {
            _puzzleStates[key] = state;
            return true;
        }

        state = IPuzzle.PuzzleState.Unsolved;
        return false;
    }

    public int GetPuzzleCountInScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return 0;

        string prefix = $"{sceneName}::";
        int count = 0;

        foreach (string key in _puzzleStates.Keys)
        {
            if (key.StartsWith(prefix))
                count++;
        }

        return count;
    }

    public int GetSolvedPuzzleCountInScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return 0;

        string prefix = $"{sceneName}::";
        int count = 0;

        foreach (KeyValuePair<string, IPuzzle.PuzzleState> entry in _puzzleStates)
        {
            if (!entry.Key.StartsWith(prefix))
                continue;

            if (entry.Value == IPuzzle.PuzzleState.Solved)
                count++;
        }

        return count;
    }

    public bool AreAllPuzzlesSolvedInScene(string sceneName, bool requireAtLeastOnePuzzle = true)
    {
        int totalCount = GetPuzzleCountInScene(sceneName);
        int solvedCount = GetSolvedPuzzleCountInScene(sceneName);

        if (totalCount == 0)
            return !requireAtLeastOnePuzzle;

        return solvedCount == totalCount;
    }


    public void ResetAllPuzzles()
    {
        foreach (KeyValuePair<string, IPuzzle> entry in _registeredPuzzles)
        {
            IPuzzle puzzle = entry.Value;

            if (puzzle == null)
                continue;

            puzzle.ResetPuzzle();

            if (TryGetPuzzleIdentity(puzzle, out string sceneName, out string puzzleId))
            {
                // Sync manager/save state after the puzzle resets itself
                UpdatePuzzleState(sceneName, puzzleId, puzzle.State);
            }
        }

        Debug.Log("PuzzleManager reset all registered puzzles.");
    }
}
