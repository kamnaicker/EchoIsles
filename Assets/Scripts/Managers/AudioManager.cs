using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public MusicTrack MusicTrack;
    public AudioState State;
    public event System.Action<AudioState> OnMusicChanged;
    private bool _isSubscribedToGameManager;
    private bool _isSubscribedToPuzzleManager;
    private bool _isSubscribedToLevelManager;

    private AudioSource _musicSource;
    private AudioConfig _audioConfig;
    private float _volume = 0.75f;

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp01(value);
            if (_musicSource != null)
                _musicSource.volume = _volume;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _volume;

            _audioConfig = Resources.Load<AudioConfig>("AudioConfig");
            if (_audioConfig == null)
                Debug.LogWarning("AudioManager: AudioConfig not found in Resources folder.");
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        TrySubscribeToGameManager();
        TrySubscribeToPuzzleManager();
        TrySubscribeToLevelManager();
    }

    private void Start()
    {
        TrySubscribeToGameManager();
        TrySubscribeToPuzzleManager();
        TrySubscribeToLevelManager();
    }

    private void OnDisable()
    {
        if (_isSubscribedToGameManager && GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

        if (_isSubscribedToPuzzleManager && PuzzleManager.Instance != null)
            PuzzleManager.Instance.OnPuzzleStateChanged -= HandlePuzzleStateChanged;

        if (_isSubscribedToLevelManager && LevelManager.Instance != null)
            LevelManager.Instance.OnLevelStateChanged -= HandleLevelStateChanged;

        _isSubscribedToGameManager = false;
        _isSubscribedToPuzzleManager = false;
        _isSubscribedToLevelManager = false;
    }

    private void TrySubscribeToGameManager()
    {
        if (_isSubscribedToGameManager || GameManager.Instance == null)
            return;

        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _isSubscribedToGameManager = true;

        if (GameManager.Instance.HasStateInitialized)
            HandleGameStateChanged(GameManager.Instance.State);
    }

    private void TrySubscribeToPuzzleManager()
    {
        if (_isSubscribedToPuzzleManager || PuzzleManager.Instance == null)
            return;

        PuzzleManager.Instance.OnPuzzleStateChanged += HandlePuzzleStateChanged;
        _isSubscribedToPuzzleManager = true;

        if (PuzzleManager.Instance.HasStateInitialized)
            HandlePuzzleStateChanged(PuzzleManager.Instance.State);
    }

    private void TrySubscribeToLevelManager()
    {
        if (_isSubscribedToLevelManager || LevelManager.Instance == null)
            return;

        LevelManager.Instance.OnLevelStateChanged += HandleLevelStateChanged;
        _isSubscribedToLevelManager = true;

        if (LevelManager.Instance.HasStateInitialized)
            HandleLevelStateChanged(LevelManager.Instance.State);
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

    public void HandleLevelStateChanged(LevelState newState)
    {
        Debug.Log($"AudioManager received LevelState: {newState}");
        switch (newState)
        {
            case LevelState.InProgress:
                break;
            case LevelState.Completed:
                PlayLevelCompleteMusic();
                break;
            case LevelState.Failed:
                break;
            case LevelState.None:
                break;
        }
    }

    public void HandleGameStateChanged(GameState newState)
    {
        Debug.Log($"AudioManager received GameState: {newState}");
        switch (newState)
        {
            case GameState.Initialisation:
                break;
            case GameState.Menu:
                PlayMenuMusic();
                break;
            case GameState.Loading:
                break;
            case GameState.Playing:
                PlaySceneMusic();
                break;
            case GameState.Paused:
                break;
            case GameState.LevelCompleted:
                PlayLevelCompleteMusic();
                break;
            case GameState.GameOver:
                break;
            case GameState.Credits:
                break;
            default:
                break;
        }
    }

    public void PlayMenuMusic()
    {
        if (_audioConfig != null)
            PlayMusic(_audioConfig.MenuMusic);
    }

    public void PlayPuzzleSolvedMusic()
    {
        if (_audioConfig != null && _audioConfig.PuzzleSolvedClip != null)
            _musicSource.PlayOneShot(_audioConfig.PuzzleSolvedClip, _volume);
    }

    private void PlayLevelCompleteMusic()
    {
        if (_audioConfig != null && _audioConfig.LevelCompleteClip != null)
            _musicSource.PlayOneShot(_audioConfig.LevelCompleteClip, _volume);
    }

    private void PlaySceneMusic()
    {
        if (_audioConfig == null)
            return;

        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip clip = _audioConfig.GetSceneClip(sceneName);
        PlayMusic(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            _musicSource.Stop();
            return;
        }

        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.Play();
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
