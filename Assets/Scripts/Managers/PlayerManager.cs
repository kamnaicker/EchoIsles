using UnityEngine;

public class PlayerManager : MonoBehaviour
{
   public static PlayerManager Instance;

    public PlayerState State;

    public event System.Action<PlayerState> OnPlayerStateChanged;
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
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    public void UpdatePlayerState(PlayerState newPlayerState)
    {
        State = newPlayerState;
        switch (newPlayerState)
        {
            case PlayerState.Idle:
                // Handle idle-specific logic here if needed
                break;
            case PlayerState.Walking:
                // Handle walking-specific logic here if needed
                break;
            case PlayerState.Running:
                // Handle running-specific logic here if needed
                break;
            case PlayerState.Jumping:
                // Handle jumping-specific logic here if needed
                break;
            case PlayerState.Falling:
                // Handle falling-specific logic here if needed
                break;
            case PlayerState.Interacting:
                // Handle interacting-specific logic here if needed
                break;
            case PlayerState.Attacking:
                // Handle attacking-specific logic here if needed
                break;
            case PlayerState.Dead:
                // Handle dead-specific logic here if needed
                break;
        }
        OnPlayerStateChanged?.Invoke(newPlayerState);
        Debug.Log($"Player state updated to: {newPlayerState}");
    }

    public void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Initialisation:
                // Handle any player-specific logic for initialisation if needed
                break;
            case GameState.Menu:
                // Handle any player-specific logic for menu if needed
                break;
            case GameState.Loading:
                // Handle any player-specific logic for loading if needed
                break;
            case GameState.Playing:
                // Handle any player-specific logic for playing if needed
                break;
            case GameState.Paused:
                // Handle any player-specific logic for paused if needed
                break;
            case GameState.LevelCompleted:
                // Handle any player-specific logic for level completed if needed
                break;
            case GameState.GameOver:
                // Handle any player-specific logic for game over if needed
                break;
            case GameState.Credits:
                // Handle any player-specific logic for credits if needed
                break;
        }
    }
}

public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Interacting,
    /* TODO: Unlikely to include this this state, due to no enemies in scope. 
     * Ask Team if we want Attacking State.
     * Avoid Scope Creep by adding it now, 
     * but if we do want it, we can add it in.*/
    Attacking,
    Dead
}