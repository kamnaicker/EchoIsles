using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class Replayer : MonoBehaviour
{
	public static Replayer Instance;

	[SerializeField] private UIDocument guiDocument;
	public Transform player;
	public Transform echo;
	[SerializeField] private bool recording = false;
	[SerializeField] private bool replaying = false;

	public float maxRecordingDuration = 10f;
	public float remainingDuration = 10f;

	public float remainingDurationPercentage = 100f;
	public bool isRecordingAvailable = false;

	private List<Vector3> _positions;
	private ProgressBar _remainingEchoBar;
	private VisualElement _replayIcon;

	private void OnEnable()
	{
		VisualElement root = guiDocument.rootVisualElement;
		_remainingEchoBar = root.Q<ProgressBar>("remaining-echo-bar");
		_replayIcon = root.Q<VisualElement>("replay-icon");

		_replayIcon.style.opacity = 0.5f;
		_remainingEchoBar.style.opacity = 0.5f;

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
			return;
		}

		echo.position = _positions.LastOrDefault();
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
		{
			_replayIcon.style.opacity = 1f;
			_remainingEchoBar.style.opacity = 1f;
		}
		else
		{
			_replayIcon.style.opacity = 0.5f;
			_remainingEchoBar.style.opacity = 0.5f;
		}
	}
}