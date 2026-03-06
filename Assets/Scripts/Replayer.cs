using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Replayer : MonoBehaviour
{
    public static Replayer Instance;

    [SerializeField] private UIDocument guiDocument;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerEchoPrefab;
    [SerializeField] private bool recording = false;
    [SerializeField] private bool replaying = false;

    public float maxRecordingDuration = 10f;
    public float remainingDuration = 10f;

    public float remainingDurationPercentage = 100f;
    public bool isRecordingAvailable = false;

    private GameObject _echoInstance = null;
    private Rigidbody _echoInstanceRigidBody = null;
    private Vector3 _echoDefaultPosition = new(0, 100, 0);
    private List<Vector3> _positions;
    private ProgressBar _remainingEchoBar;
    private Image _replayIcon;
    private Image _recordIcon;
    private Image _playIcon;
    private VisualElement _echoBarContainer;

    private VisualElement _recordButtonHint;
    private VisualElement _replayButtonHint;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        TryBindUiReferences();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Instance.ApplySceneReferencesFrom(this);
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        _positions = new List<Vector3>();
        EnsureEchoInstance();
        ResetReplayState(clearPositions: true);

        if (gameObject.GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
    }


    private void FixedUpdate()
    {
        if (recording) Record();

        if (replaying) Replay();
    }

    private void Record()
    {
        if (!EnsurePlayerReference())
        {
            recording = false;
            return;
        }

        if (remainingDuration <= 0f)
        {
            remainingDuration = 0;
            remainingDurationPercentage = 0;
            recording = false;
            return;
        }

        _positions.Insert(0, player.position);
        remainingDuration -= Time.fixedDeltaTime;
        remainingDurationPercentage = remainingDuration / maxRecordingDuration * 100f;
    }

    private void Replay()
    {
        if (!EnsureEchoInstance())
        {
            replaying = false;

            AudioManager.Instance.HandleEchoStateChanged(Echo_State.Inactive, gameObject.GetComponent<AudioSource>());

            return;
        }

        if (_positions.Count == 0)
        {
            ResetReplayState(clearPositions: false);
            MoveEchoToDefaultPosition();

            AudioManager.Instance.HandleEchoStateChanged(Echo_State.Inactive, gameObject.GetComponent<AudioSource>());

            return;
        }


        // _echoInstance.transform.position = _positions.LastOrDefault();
        _echoInstanceRigidBody.MovePosition(_positions.LastOrDefault());
        _positions.RemoveAt(_positions.Count - 1);

        remainingDuration += Time.fixedDeltaTime;
        remainingDurationPercentage = remainingDuration / maxRecordingDuration * 100f;
    }

    private void BindEchoProgressBarUI()
    {
        if (_remainingEchoBar == null)
            return;

        _remainingEchoBar.dataSource = this;

        _remainingEchoBar.SetBinding("value", new DataBinding
        {
            dataSourcePath = new PropertyPath(nameof(remainingDurationPercentage)),
            bindingMode = BindingMode.ToTarget
        });
    }

    public void ToggleRecordingAvailability(bool isAvailable)
    {
        TryBindUiReferences();
        isRecordingAvailable = isAvailable;
        if (isAvailable)
        {
            if (_echoBarContainer != null)
                _echoBarContainer.style.opacity = 1f;

            if (_recordButtonHint != null)
                _recordButtonHint.style.opacity = 1f;
        }
        else if (!recording)
        {
            if (_recordButtonHint != null)
                _recordButtonHint.style.opacity = 0.5f;

            if (_echoBarContainer != null)
                _echoBarContainer.style.opacity = 0.5f;
        }
    }

    public void OnRecord()
    {
        Debug.Log("Record");
        TryBindUiReferences();

        if (_replayIcon != null)
            _replayIcon.style.display = DisplayStyle.None;

        if (_recordIcon != null)
            _recordIcon.style.display = DisplayStyle.Flex;


        if (!recording && isRecordingAvailable)
        {
            recording = true;

            AudioManager.Instance.HandleEchoStateChanged(Echo_State.Recording, gameObject.GetComponent<AudioSource>());

            return;
        }

        if (recording)
        {
            recording = false;

            if (_replayButtonHint != null)
                _replayButtonHint.style.opacity = 1f;

            AudioManager.Instance.HandleEchoStateChanged(Echo_State.Inactive, gameObject.GetComponent<AudioSource>());
        }
    }

    public void OnReplay()
    {
        Debug.Log("Replay");
        TryBindUiReferences();

        if (_recordIcon != null)
            _recordIcon.style.display = DisplayStyle.None;

        if (_playIcon != null)
            _playIcon.style.display = DisplayStyle.Flex;

        if (!recording)
        {
            replaying = true;

            if (_replayButtonHint != null)
                _replayButtonHint.style.opacity = 0.5f;

            AudioManager.Instance.HandleEchoStateChanged(Echo_State.Playback, gameObject.GetComponent<AudioSource>());
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBindUiReferences();
        ResetReplayState(clearPositions: true);
        EnsurePlayerReference();
        EnsureEchoInstance();
        MoveEchoToDefaultPosition();
    }

    private void TryBindUiReferences()
    {
        if (guiDocument == null)
            return;

        VisualElement root = guiDocument.rootVisualElement;
        if (root == null)
            return;

        _echoBarContainer = root.Q<VisualElement>("replay-elements");
        _remainingEchoBar = root.Q<ProgressBar>("remaining-echo-bar");
        _replayIcon = root.Q<Image>("replay-icon");
        _playIcon = root.Q<Image>("play-icon");
        _recordIcon = root.Q<Image>("record-icon");

        _recordButtonHint = root.Q<VisualElement>("record-button");
        _replayButtonHint = root.Q<VisualElement>("replay-button");

        if (_echoBarContainer != null)
            _echoBarContainer.style.opacity = 0.5f;

        if (_recordButtonHint != null)
            _recordButtonHint.style.opacity = 0.5f;

        if (_replayButtonHint != null)
            _replayButtonHint.style.opacity = 0.5f;

        BindEchoProgressBarUI();
    }

    private void ApplySceneReferencesFrom(Replayer source)
    {
        if (source == null)
            return;

        if (source.guiDocument != null)
            guiDocument = source.guiDocument;

        if (source.player != null)
            player = source.player;

        if (source.playerEchoPrefab != null)
            playerEchoPrefab = source.playerEchoPrefab;

        TryBindUiReferences();
    }

    private bool EnsurePlayerReference()
    {
        if (player != null)
            return true;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
            return false;

        player = playerObject.transform;
        return player != null;
    }

    private bool EnsureEchoInstance()
    {
        if (_echoInstanceRigidBody != null)
            return true;

        if (_echoInstance != null)
            _echoInstanceRigidBody = _echoInstance.GetComponent<Rigidbody>();

        if (_echoInstanceRigidBody != null)
            return true;

        if (playerEchoPrefab == null)
        {
            Debug.LogWarning("Replayer cannot spawn echo because playerEchoPrefab is missing.");
            return false;
        }

        _echoInstance = Instantiate(playerEchoPrefab, _echoDefaultPosition, Quaternion.identity);
        _echoInstanceRigidBody = _echoInstance.GetComponent<Rigidbody>();
        if (_echoInstanceRigidBody == null)
        {
            Debug.LogWarning("Replayer echo prefab is missing a Rigidbody component.");
            return false;
        }

        DontDestroyOnLoad(_echoInstance);
        return true;
    }

    private void MoveEchoToDefaultPosition()
    {
        if (_echoInstanceRigidBody == null)
            return;

        _echoInstanceRigidBody.MovePosition(_echoDefaultPosition);
    }

    private void ResetReplayState(bool clearPositions)
    {
        recording = false;
        replaying = false;
        isRecordingAvailable = false;
        remainingDuration = maxRecordingDuration;
        remainingDurationPercentage = 100f;

        if (clearPositions)
            _positions?.Clear();

        if (_echoBarContainer != null)
            _echoBarContainer.style.opacity = 0.5f;

        if (_recordButtonHint != null)
            _recordButtonHint.style.opacity = 0.5f;

        if (_replayButtonHint != null)
            _replayButtonHint.style.opacity = 0.5f;

        if (_playIcon != null)
            _playIcon.style.display = DisplayStyle.None;

        if (_recordIcon != null)
            _recordIcon.style.display = DisplayStyle.None;

        if (_replayIcon != null)
            _replayIcon.style.display = DisplayStyle.Flex;
    }
}