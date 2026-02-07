using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;
    public InteractionState State;
    public event System.Action<InteractionState> OnInteractionStateChanged;

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

    public void UpdateInteractionState(InteractionState newInteractionState)
    {
        State = newInteractionState;
        switch (newInteractionState)
        {
            case InteractionState.None:
                break;
            case InteractionState.Pickup:
                break;
            case InteractionState.Talk:
                break;
            case InteractionState.Examine:
                break;
            case InteractionState.Use:
                break;
            case InteractionState.Push:
                break;
            case InteractionState.Pull:
                break;
        }
        OnInteractionStateChanged?.Invoke(newInteractionState);
        Debug.Log($"Interaction State Changed to {newInteractionState}");
    }

    public void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Initialisation:
                // Handle any interaction-specific logic for initialisation if needed
                break;
            case GameState.Menu:
                // Handle any interaction-specific logic for menu if needed
                break;
            case GameState.Loading:
                // Handle any interaction-specific logic for loading if needed
                break;
            case GameState.Playing:
                // Handle any interaction-specific logic for playing if needed
                break;
            case GameState.Paused:
                // Handle any interaction-specific logic for paused if needed
                break;
            case GameState.LevelCompleted:
                // Handle any interaction-specific logic for level completed if needed
                break;
            case GameState.GameOver:
                // Handle any interaction-specific logic for game over if needed
                break;
            case GameState.Credits:
                // Handle any interaction-specific logic for credits if needed
                break;
        }
    }
}

public enum InteractionState
{
    None,
    Pickup,
    Talk,
    Examine,
    Use,
    Push,
    Pull,
    //TODO: Move to PuzzleManager if these are more puzzle related than interaction related
    PressedPressurePlate,
    DepressedPressurePlate,
    DoorsOpened,
    DoorsClosed,
    TrapActivated,
    TrapDeactivated,
    ElevatorActivated,
    ElevatorDeactivated,
    PlatformActivated,
    PlatformDestroyed,
    PlatformDeactivated,
    BlockMoved,
    BlockPlaced,
    BlockFell,
    TorchLit,
    TorchExtinguished    
}