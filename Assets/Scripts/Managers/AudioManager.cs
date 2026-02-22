using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public MusicTrack MusicTrack;
    public AudioState State;
    public event System.Action<AudioState> OnMusicChanged;
    private bool _isSubscribedToGameManager;
    private bool _isSubscribedToPuzzleManager;

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

    void OnEnable()
    {
        Debug.Log("AudioManager enabled, trying to subscribe to GameManager and PuzzleManager events.");
        Debug.Log($"Trying to subscribe to GameManager events. Result is: {_isSubscribedToGameManager}");
        TrySubscribeToGameManager();
        Debug.Log($"Trying to subscribe to GameManager events. Result is: {_isSubscribedToGameManager}");
        Debug.Log($"Trying to subscribe to PuzzleManager events. Result is: {_isSubscribedToPuzzleManager}");
        TrySubscribeToPuzzleManager();
        Debug.Log($"Trying to subscribe to PuzzleManager events. Result is: {_isSubscribedToPuzzleManager}");
    }

    private void Start()
    {
        Debug.Log("AudioManager started, trying to subscribe to GameManager and PuzzleManager events.");
        Debug.Log($"Trying to subscribe to GameManager events. Result is: {_isSubscribedToGameManager}");
        TrySubscribeToGameManager();
        Debug.Log($"Trying to subscribe to GameManager events. Result is: {_isSubscribedToGameManager}");
        Debug.Log($"Trying to subscribe to PuzzleManager events. Result is: {_isSubscribedToPuzzleManager}");
        TrySubscribeToPuzzleManager();
        Debug.Log($"Trying to subscribe to PuzzleManager events. Result is: {_isSubscribedToPuzzleManager}");
    }

    void OnDisable()
    {
        Debug.Log("AudioManager disabled, unsubscribing from GameManager and PuzzleManager events.");

        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        if(_isSubscribedToPuzzleManager && PuzzleManager.Instance != null)
            PuzzleManager.Instance.OnPuzzleStateChanged -= HandlePuzzleStateChanged;

        _isSubscribedToGameManager = false;
        _isSubscribedToPuzzleManager = false;
    }

    private void TrySubscribeToGameManager()
    {
        if (_isSubscribedToGameManager || GameManager.Instance == null)
            return;

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _isSubscribedToGameManager = true;
        Debug.Log("AudioManager subscribed to GameManager.OnGameStateChanged.");

        if (GameManager.Instance.HasStateInitialized)
        {
            Debug.Log($"AudioManager late-syncing to current GameState: {GameManager.Instance.State}");
            HandleGameStateChanged(GameManager.Instance.State);
        }
    }
    private void TrySubscribeToPuzzleManager()
    {
        if (_isSubscribedToPuzzleManager || PuzzleManager.Instance == null)
            return;

        PuzzleManager.Instance.OnPuzzleStateChanged += HandlePuzzleStateChanged;
        _isSubscribedToPuzzleManager = true;
        Debug.Log("AudioManager subscribed to PuzzleManager.OnPuzzleStateChanged.");

        if (PuzzleManager.Instance.HasStateInitialized)
        {
            Debug.Log($"AudioManager late-syncing to current PuzzleState: {PuzzleManager.Instance.State}");
            HandlePuzzleStateChanged(PuzzleManager.Instance.State);
        }
    }

    public void UpdateAudioState(AudioState newAudioState)
    {
        State = newAudioState;

        switch (State)
        {
            case AudioState.Playing:
                // Handle audio for playing state
                break;
            case AudioState.Paused:
                // Handle audio for paused state
                break;
            case AudioState.Muted:
                // Handle audio for muted state
                break;
            case AudioState.FadingIn:
                // Handle audio for fading in state
                break;
            case AudioState.FadingOut:
                // Handle audio for fading out state
                break;
            default:
                break;
        }

        OnMusicChanged?.Invoke(newAudioState);
        Debug.Log($"Audio state updated to: {State}");
    }

    public void HandlePuzzleStateChanged(IPuzzle.PuzzleState newState)
    {
        Debug.Log($"AudioManager received PuzzleState: {newState}");
        switch (newState)
        {
            case IPuzzle.PuzzleState.Unsolved:
                // Handle audio for unsolved puzzle
                break;
            case IPuzzle.PuzzleState.Solved:
                // Handle audio for solved puzzle
                PlayPuzzleSolvedMusic();
                break;
            default:
                break;
        }
    }

    public void HandleGameStateChanged(GameState newState)
    {
        Debug.Log($"AudioManager received GameState: {newState}");
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
    

public void PlayMenuMusic()
    {
        Debug.Log("Playing menu music");
    }
    public void PlayGameplayMusic()
    {
        Debug.Log("Playing gameplay music");
    }

    public void PlayPuzzleSolvedMusic()
    {
        Debug.Log("Playing Puzzle Solved Music");
    }
}

    

public enum MusicTrack
{
    None,
    MainMenu,
    Gameplay,
    Victory,
    GameOver,
    Credits,
    Ambient
}

public enum AudioState
{
    Playing,
    Paused,
    Muted,
    FadingIn,
    FadingOut
}
