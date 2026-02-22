using System;
using UnityEngine;

public class EchoManager : MonoBehaviour
{
    public static EchoManager Instance;
    public EchoState State;
    public event Action<EchoState> OnEchoStateChanged;
    private bool _isSubscribedToGameManager;

    void Awake()
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
        Debug.Log("EchoManager OnEnable -> attempting GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void Start()
    {
        Debug.Log("EchoManager Start -> retrying GameManager subscription.");
        TrySubscribeToGameManager();
    }

    private void OnDisable()
    {
        Debug.Log($"EchoManager OnDisable -> subscribed={_isSubscribedToGameManager}");
        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        _isSubscribedToGameManager = false;
    }

    private void TrySubscribeToGameManager()
    {
        if (_isSubscribedToGameManager)
        {
            Debug.Log("EchoManager subscription skipped (already subscribed).");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.Log("EchoManager subscription deferred (GameManager.Instance is null).");
            return;
        }

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _isSubscribedToGameManager = true;
        Debug.Log("EchoManager subscribed to GameManager.OnGameStateChanged.");

        if (GameManager.Instance.HasStateInitialized)
        {
            Debug.Log($"EchoManager late-syncing to current GameState: {GameManager.Instance.State}");
            HandleGameStateChanged(GameManager.Instance.State);
        }
    }

    public void UpdateEchoState(EchoState newEchoState)
    {
        Debug.Log($"In UpdateEchoState, got new state: {newEchoState}");
        State = newEchoState;

        switch (newEchoState) 
        {
            case EchoState.Available:
                break;
            case EchoState.Unavailable:
                break;
            case EchoState.Spawned:
                break;
            case EchoState.Destroyed:
                break;
            case EchoState.Focused:
                break;
            case EchoState.Recording:
                break;
            case EchoState.Replaying:
                break;

        }

        OnEchoStateChanged?.Invoke(newEchoState);
        Debug.Log($"Echo State Changed to {newEchoState}");
    }

    public void HandleGameStateChanged(GameState newState)
    {
        Debug.Log($"EchoManager received GameState: {newState}");
        switch (newState)
        {
            case GameState.Initialisation:
                // Handle audio for initialisation
                break;
            case GameState.Menu:
                // Handle audio for menu
                break;
            case GameState.Loading:
                // Handle audio for loading
                break;
            case GameState.Playing:
                // Handle audio for playing
                break;
            case GameState.Paused:
                // Handle audio for paused
                break;
            case GameState.LevelCompleted:
                // Handle audio for level completed
                break;
            case GameState.GameOver:
                // Handle audio for game over
                break;
            case GameState.Credits:
                // Handle audio for credits
                break;
            default:
                break;
        }
    }   
}

public enum EchoState
{
    Available,
    Unavailable,
    Spawned,
    Destroyed,
    Focused,
    Recording,
    Replaying,
}
