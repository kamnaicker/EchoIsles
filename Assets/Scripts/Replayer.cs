using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
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
	private List<Vector3> _positions;
	private ProgressBar _remainingEchoBar;
	private Image _replayIcon;
	private Image _recordIcon;
	private Image _playIcon;
	private VisualElement _echoBarContainer;

	private void OnEnable()
	{
		VisualElement root = guiDocument.rootVisualElement;

		_echoBarContainer = root.Q<VisualElement>("replay-elements");
		_remainingEchoBar = root.Q<ProgressBar>("remaining-echo-bar");
		_replayIcon = root.Q<Image>("replay-icon");
		_playIcon = root.Q<Image>("play-icon");
		_recordIcon = root.Q<Image>("record-icon");

		_echoBarContainer.style.opacity = 0.5f;

		BindEchoProgressBarUI();
	}

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void Start()
	{
		DontDestroyOnLoad(gameObject);
		_positions = new List<Vector3>();
	}


	private void FixedUpdate()
	{
		if (recording) Record();

		if (replaying) Replay();
	}

	private void Record()
	{
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
		if (_positions.Count == 0)
		{
			replaying = false;
			remainingDuration = 10f;
			remainingDurationPercentage = 100f;

			_echoBarContainer.style.opacity = 0.5f;
			_playIcon.style.display = DisplayStyle.None;
			_replayIcon.style.display = DisplayStyle.Flex;

			if (_echoInstance is not null)
			{
				Destroy(_echoInstance);
				_echoInstance = null;
			}

			return;
		}

		_echoInstance ??= Instantiate(playerEchoPrefab, _positions.LastOrDefault(), Quaternion.identity);

		_echoInstance.transform.position = _positions.LastOrDefault();
		_positions.RemoveAt(_positions.Count - 1);

		remainingDuration += Time.fixedDeltaTime;
		remainingDurationPercentage = remainingDuration / maxRecordingDuration * 100f;
	}

	private void BindEchoProgressBarUI()
	{
		_remainingEchoBar.dataSource = this;

		_remainingEchoBar.SetBinding("value", new DataBinding
		{
			dataSourcePath = new PropertyPath(nameof(remainingDurationPercentage)),
			bindingMode = BindingMode.ToTarget
		});
	}

	public void ToggleRecordingAvailability(bool isAvailable)
	{
		isRecordingAvailable = isAvailable;
		if (isAvailable)
			_echoBarContainer.style.opacity = 1f;
		else if (!recording) _echoBarContainer.style.opacity = 0.5f;
	}

	public void OnRecord()
	{
		Debug.Log("Record");

		_replayIcon.style.display = DisplayStyle.None;
		_recordIcon.style.display = DisplayStyle.Flex;


		if (!recording && isRecordingAvailable)
		{
			recording = true;
			return;
		}

		if (recording) recording = false;
	}

	public void OnReplay()
	{
		Debug.Log("Replay");

		_recordIcon.style.display = DisplayStyle.None;
		_playIcon.style.display = DisplayStyle.Flex;

		if (!recording) replaying = true;
	}
}