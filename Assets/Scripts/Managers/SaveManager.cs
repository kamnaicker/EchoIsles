using System;
using System.Collections.Generic;
using System.IO;
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
    //set default file path to root of persistent data path, but can be overridden in inspector for testing purposes.
    public string SaveFileName = string.Empty;

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
        Debug.Log($"Save state changed to {newSaveState}");
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
        CurrentSave = new SaveData()
        {
            saveVersion = SaveVersion,
            lastSavedUtcIso = DateTime.UtcNow.ToString("O"),
            currentSceneName = "",
            unlockedLevelIds = new List<string>(),
            completedLevelIds = new List<string>(),
            puzzleStates = new List<PuzzleSaveRecord>()
        };
    }

    public bool HasSaveFile()
    {
        string path = ResolveSaveFilePath();

        return System.IO.File.Exists(path);
    }

    public bool LoadGame()
    {
        try
        {
            string path = ResolveSaveFilePath();

            if (!System.IO.File.Exists(path))
            {
                LastError = "Save file not found.";

                if (LogSaveOperations)
                    Debug.Log($"SaveManager LoadGame failed: {LastError} Path: {path}");

                return false;
            }

            UpdateSaveState(SaveState.Loading);

            string json = System.IO.File.ReadAllText(path);

            if (string.IsNullOrEmpty(json))
            {
                LastError = "Save file is empty.";
                UpdateSaveState(SaveState.Error);
                return false;
            }

            SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

            if (loadedData == null)
            {
                LastError = "Failed to parse save file.";
                UpdateSaveState(SaveState.Error);
                return false;
            }

            CurrentSave = loadedData;
            EnsureSaveInitialised();

            HasLoadedSave = true;
            LastError = string.Empty;
            UpdateSaveState(SaveState.Saved);

            if (LogSaveOperations)
                Debug.Log($"SaveManager loaded file from: {path}");

            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            UpdateSaveState(SaveState.Error);
            Debug.LogError($"SaveManager LoadGame failed: {LastError}");

            return false;
        }
    }

    public bool SaveGame()
    {
        try
        {
            EnsureSaveInitialised();

            CurrentSave.saveVersion = SaveVersion;
            CurrentSave.lastSavedUtcIso = DateTime.UtcNow.ToString("O");

            string path = ResolveSaveFilePath();
            string json = JsonUtility.ToJson(CurrentSave, true);

            System.IO.File.WriteAllText(path, json);

            HasLoadedSave = true;
            LastError = null;
            UpdateSaveState(SaveState.Saved);

            if (LogSaveOperations)
                Debug.Log($"SaveManager saved file to: {path}");

            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            UpdateSaveState(SaveState.Error);
            Debug.LogError($"SaveManager SaveGame failed: {ex}");
            return false;
        }
    }

    public bool DeleteSave()
    {
        try
        {
            string path = ResolveSaveFilePath();

            UpdateSaveState(SaveState.Deleting);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            ResetRuntimeSaveData();
            LastError = null;

            if (LogSaveOperations)
                Debug.Log($"SaveManager deleted save file at: {path}");

            return true;

        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            UpdateSaveState(SaveState.Error);
            if (LogSaveOperations)
                Debug.Log($"SaveManager delete failed: {LastError}");

            return false;
        }

    }

    public void SetCurrentScene(string sceneName)
    {
        EnsureSaveInitialised();

        if (string.IsNullOrEmpty(sceneName))
            return;

        CurrentSave.currentSceneName = sceneName;
    }

    public void MarkLevelUnlocked(string levelId)
    {
        EnsureSaveInitialised();

        if (string.IsNullOrEmpty(levelId))
            return;

        if (!CurrentSave.unlockedLevelIds.Contains(levelId))
            CurrentSave.unlockedLevelIds.Add(levelId);
    }

    public void MarkLevelCompleted(string levelId)
    {

        EnsureSaveInitialised();

        if (string.IsNullOrEmpty(levelId))
            return;

        if (!CurrentSave.completedLevelIds.Contains(levelId))
            CurrentSave.completedLevelIds.Add(levelId);
    }

    public void SetPuzzleState(string sceneName, string puzzleId, IPuzzle.PuzzleState state)
    {
        EnsureSaveInitialised();

        if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(puzzleId))
            return;

        PuzzleSaveRecord record = FindPuzzleRecord(sceneName, puzzleId);

        if (record == null)
        {
            record = new PuzzleSaveRecord
            {

                SceneName = sceneName,
                PuzzleId = puzzleId,
                State = state
            };

            CurrentSave.puzzleStates.Add(record);
        }
        else
        {

            record.State = state;

        }
    }

    public bool TryGetPuzzleState(string sceneName, string puzzleId, out IPuzzle.PuzzleState state)
    {
        PuzzleSaveRecord record = FindPuzzleRecord(sceneName, puzzleId);

        if (record != null)
        {
            state = record.State;
            return true;
        }

        state = IPuzzle.PuzzleState.Unsolved;

        return false;
    }

    public void ResetRuntimeSaveData()
    {
        CurrentSave = null;
        HasLoadedSave = false;
        LastError = null;
        InitialiseNewSave();
    }

    private string BuildSaveFilePath(string fileName)
    {
        return System.IO.Path.Combine(Application.persistentDataPath, fileName);
    }

    private string ResolveSaveFilePath()
    {
        if (string.IsNullOrWhiteSpace(SaveFileName))
            SaveFileName = $"save_{SaveVersion}.json";
        else
            SaveFileName = SaveFileName.Trim();

        SavePath = BuildSaveFilePath(SaveFileName);

        return SavePath;
    }

    private void EnsureSaveInitialised()
    {
        if (CurrentSave == null)
            InitialiseNewSave();

        if (CurrentSave.puzzleStates == null || CurrentSave.puzzleStates.Count == 0)
            CurrentSave.puzzleStates = new List<PuzzleSaveRecord>();

        if (CurrentSave.unlockedLevelIds == null || CurrentSave.unlockedLevelIds.Count == 0)
            CurrentSave.unlockedLevelIds = new List<string>();

        if (CurrentSave.completedLevelIds == null || CurrentSave.completedLevelIds.Count == 0)
            CurrentSave.completedLevelIds = new List<string>();

        if (string.IsNullOrEmpty(CurrentSave.currentSceneName))
            CurrentSave.currentSceneName = "";
    }

    private PuzzleSaveRecord FindPuzzleRecord(string sceneName, string puzzleId)
    {
        EnsureSaveInitialised();

        if (string.IsNullOrWhiteSpace(sceneName) || string.IsNullOrWhiteSpace(puzzleId))
            return null;

        return CurrentSave.puzzleStates.Find(record => record.SceneName == sceneName && record.PuzzleId == puzzleId);
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
