using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public MusicTrack MusicTrack;
    public AudioState State;
    public event System.Action<AudioState> OnMusicChanged;
    private bool _isSubscribedToGameManager;
    private bool _isSubscribedToPuzzleManager;

    private AudioSource windSource;

    [field: SerializeField]
    public AudioClip footsteps { get; set; }

    [field: SerializeField]
    public AudioClip wind { get; set; }

    [field: SerializeField]
    public AudioClip door_open { get; set; }
    [field: SerializeField]
    public AudioClip door_close { get; set; }

    [field: SerializeField]
    public AudioClip pressure_plate_down { get; set; }
    [field: SerializeField]
    public AudioClip pressure_plate_up { get; set; }

    [field: SerializeField]
    public AudioClip echo_area { get; set; }
    [field: SerializeField]
    public AudioClip echo_area_activated { get; set; }

    [field: SerializeField]
    public AudioClip echo_playback { get; set; }
    [field: SerializeField]
    public AudioClip echo_recording { get; set; }

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

        windSource = gameObject.AddComponent<AudioSource>();
        windSource.clip = wind;
        windSource.loop = true;
    }

    void OnDisable()
    {
        Debug.Log("AudioManager disabled, unsubscribing from GameManager and PuzzleManager events.");

        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        if (_isSubscribedToPuzzleManager && PuzzleManager.Instance != null)
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
                windSource.Play();
                break;
            case GameState.Paused:
                // Handle audio for paused
                windSource.Stop();
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

    public void HandleMovementStateChanged(MoveState newState, AudioSource walkSoundSource)
    {
        switch (newState)
        {
            case MoveState.Walking:
                walkSoundSource.clip = footsteps;
                walkSoundSource.loop = true;
                walkSoundSource.pitch = 0.6f;
                walkSoundSource.volume = 0.5f;
                walkSoundSource.PlayDelayed(0.2f);
                break;
            case MoveState.Standing:
                walkSoundSource.Stop();
                break;
        }
    }

    public void HandleDoorStateChanged(DoorState newState, AudioSource fxSource)
    {
        switch (newState)
        {
            case DoorState.Open:
                fxSource.volume = 0.7f;
                fxSource.PlayOneShot(door_open);
                break;
            case DoorState.Close:
                fxSource.volume = 0.7f;
                fxSource.PlayOneShot(door_close);
                break;
        }
    }

    public void HandlePressurePlateStateChanged(PressurePlateState newState, AudioSource fxSource)
    {
        switch (newState)
        {
            case PressurePlateState.Down:
                fxSource.volume = 0.8f;
                fxSource.PlayOneShot(pressure_plate_down);
                break;
            case PressurePlateState.Up:
                fxSource.volume = 0.6f;
                fxSource.PlayOneShot(pressure_plate_up);
                break;
        }
    }

    public void HandleEchoPlateStateChanged(EchoPlateState newState, AudioSource fxSource)
    {
        switch (newState)
        {
            case EchoPlateState.On:
                fxSource.clip = echo_area;
                fxSource.volume = 0.7f;
                fxSource.loop = true;
                fxSource.Play();
                break;
            case EchoPlateState.Off:
                fxSource.Stop();
                break;
            case EchoPlateState.Activated:
                fxSource.PlayOneShot(echo_area_activated);
                break;
        }
    }

    public void HandleEchoStateChanged(Echo_State newState, AudioSource fxSource)
    {
        switch (newState)
        {
            case Echo_State.Inactive:
                fxSource.Stop();
                break;
            case Echo_State.Recording:
                fxSource.clip = echo_recording;
                fxSource.volume = 0.8f;
                fxSource.loop = true;
                fxSource.Play();
                break;
            case Echo_State.Playback:
                fxSource.clip = echo_playback;
                fxSource.volume = 0.8f;
                fxSource.loop = true;
                fxSource.Play();
                break;
        }
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


public enum MoveState
{
    Walking,
    Standing
}

public enum DoorState
{
    Open,
    Close
}

public enum PressurePlateState
{
    Down,
    Up
}

public enum EchoPlateState
{
    On,
    Off,
    Activated
}

public enum Echo_State
{
    Inactive,
    Recording,
    Playback
}
