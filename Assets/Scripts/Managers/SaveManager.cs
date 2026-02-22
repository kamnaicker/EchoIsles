using System;
using System.Collections.Generic;
using UnityEngine;
using static IPuzzle;

public class SaveManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public int saveVersion;
        public string lastSavedUtcIso;
        public string currentSceneName;
        public List<string> unlockedLevelIds;
        public List<string> completedLevelIds;
        public List<PuzzleSaveRecord> puzzleStates;
    }

    [System.Serializable]
    public class PuzzleSaveRecord
    {
        public string SceneName;
        public string PuzzleId;
        public IPuzzle.PuzzleState State;
    }
    // Singleton instance
    public static SaveManager Instance;
    // Current game state
    public SaveState State;
    // Event for game state changes
    public event System.Action<SaveState> OnSaveStateChanged;

    public bool HasLoadedSave;

    public bool AutoSaveEnabled = true;

    public string SaveFileName;

    public string SavePath;

    public int SaveVersion;

    public SaveManager.SaveData CurrentSave;

    private bool _isSubscribedToGameManager;

    //debug fields
    public bool LogSaveOperations;

    public string LastError;


    void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
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
        Debug.Log("SaveManager OnEnable -> attempting GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void Start()
    {
        Debug.Log("SaveManager Start -> retrying GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void OnDisable()
    {
        Debug.Log($"SaveManager OnDisable -> subscribed={_isSubscribedToGameManager}");
        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        _isSubscribedToGameManager = false;
    }

    private void TrySubscribeToGameManager()
    {
        if (_isSubscribedToGameManager)
        {
            Debug.Log("SaveManager subscription skipped (already subscribed).");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.Log("SaveManager subscription deferred (GameManager.Instance is null).");
            return;
        }

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _isSubscribedToGameManager = true;
        Debug.Log("SaveManager subscribed to GameManager.OnGameStateChanged.");

        if (GameManager.Instance.HasStateInitialized)
        {
            Debug.Log($"SaveManager late-syncing to current GameState: {GameManager.Instance.State}");
            HandleGameStateChanged(GameManager.Instance.State);
        }
    }

    public void UpdateSaveState(SaveState newSaveState)
    {
        State = newSaveState;
        switch (newSaveState)
        {
            case SaveState.Saved:
                break;
            case SaveState.Loading:
                break;
            case SaveState.Deleting:
                break;
            case SaveState.Error:
                break;
            default:
                break;
        }
        OnSaveStateChanged?.Invoke(newSaveState);
        Debug.Log($"Interaction State Changed to {newSaveState}");
    }

    public void HandleGameStateChanged(GameState newState)
    {
        Debug.Log($"SaveManager received GameState: {newState}");
        switch (newState)
        {
            case GameState.Initialisation:
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
        Debug.Log("Game State changed from SaveManager to: " + newState.ToString());
    }

    public void InitialiseNewSave()
    {
        CurrentSave = new SaveData();
    }

    public bool HasSaveFile()
    {
        return false;
    }

    public bool LoadGame()
    {
        return false;
    }

    public bool SaveGame()
    {
        return false;
    }

    public bool DeleteSave()
    {
        return false;
    }

    //TODO: Move to a more appropriate manager, but for now it can be here for testing purposes.
    //This is to allow us to track which scene the player is currently in,
    //so we can load the correct scene when loading a save file.
    public void SetCurrentScene(string sceneName)
    {

    }

    public void MarkLevelUnlocked(string levelId) { }

    public void MarkLevelCompleted(string levelId) { }

    public void SetPuzzleState(string sceneName, string puzzleId, IPuzzle.PuzzleState state) { }

    public bool TryGetPuzzleState(string sceneName, string puzzleId, IPuzzle.PuzzleState state) { return false; }

    public void ResetRuntimeSaveData() { }

    private string BuildSaveFilePath(string fileName)
    {
        return System.IO.Path.Combine(Application.persistentDataPath, fileName);
    }

    private void EnsureSaveInitialised() 
    { 
    }

    private PuzzleSaveRecord FindPuzzleRecord(string sceneName, string puzzleId) 
    { 
        return new PuzzleSaveRecord(); 
    }

}

//Collection of possible game states. Note: Can be expanded or reduced as needed.
public enum SaveState
{
    Saved = 0,
    Loading = 1,
    Deleting = 2,
    Error = 3
}
