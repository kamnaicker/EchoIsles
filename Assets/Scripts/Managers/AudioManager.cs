using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public MusicTrack MusicTrack;
    public AudioState State;
    public event System.Action<AudioState> OnMusicChanged;

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
        if(GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        if(PuzzleManager.Instance != null)
            PuzzleManager.Instance.OnPuzzleStateChanged += HandlePuzzleStateChanged;
    }   

    void OnDisable()
    {
        
        if(GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        if(PuzzleManager.Instance != null)
            PuzzleManager.Instance.OnPuzzleStateChanged -= HandlePuzzleStateChanged;
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

    public void HandlePuzzleStateChanged(PuzzleState newState)
    {
        switch (newState)
        {
            case PuzzleState.Unsolved:
                // Handle audio for unsolved puzzle
                break;
            case PuzzleState.Solved:
                // Handle audio for solved puzzle
                PlayPuzzleSolvedMusic();
                break;
            default:
                break;
        }
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
    Boss,
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
