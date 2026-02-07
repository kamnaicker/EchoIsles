using System;
using UnityEngine;

public class EchoManager : MonoBehaviour
{
    public static EchoManager Instance;
    public EchoState State;
    public event Action<EchoState> OnEchoStateChanged;

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
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
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
