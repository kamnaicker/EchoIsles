using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SequentialTriggers : MonoBehaviour, IPuzzle
{
	[SerializeField] private string puzzleId;
	[FormerlySerializedAs("triggers")]
	[SerializeField] private List<InteractiveObjectTrigger> orderedTriggers;
	[FormerlySerializedAs("resetAfterFinalExit")]
	[SerializeField] private bool resetProgressAfterFinalExit = true;

	private int _currentProgressSteps;
	private int _lastActivatedTriggerIndex = -1;
	private bool _isPuzzleRegistered;

	public float CompletionRate;
	public bool IsComplete => orderedTriggers.Count > 0 && _currentProgressSteps >= orderedTriggers.Count;
	public bool HasCompletedOnce { get; private set; }

	public string PuzzleId => string.IsNullOrWhiteSpace(puzzleId) ? BuildDefaultPuzzleId() : puzzleId;
	public IPuzzle.PuzzleState State { get; private set; } = IPuzzle.PuzzleState.Unsolved;

	private void OnEnable()
	{
		RegisterWithPuzzleManagerIfAvailable();
		RefreshAndPublishPuzzleState();
	}

	private void Start()
	{
		for (int i = 0; i < orderedTriggers.Count; i++)
		{
			orderedTriggers[i].OnActivation += HandleTriggerActivated;
			orderedTriggers[i].OnDeactivation += HandleTriggerDeactivated;
			orderedTriggers[i].Index = i;
		}

		UpdateCompletionRate();
		RefreshAndPublishPuzzleState();
	}

	private void OnDisable()
	{
		UnregisterFromPuzzleManager();
	}

	private void HandleTriggerActivated(InteractiveObjectTrigger trigger)
	{
		if (orderedTriggers.Count == 0) return;

		// Advance only when the next expected trigger is pressed.
		if (trigger.Index == _currentProgressSteps)
		{
			_currentProgressSteps++;
			_lastActivatedTriggerIndex = trigger.Index;
		}
		// Pressing the first trigger starts/restarts a run.
		else if (trigger.Index == 0)
		{
			_currentProgressSteps = 1;
			_lastActivatedTriggerIndex = 0;
		}
		// Any other out-of-order trigger resets the sequence.
		else
		{
			_currentProgressSteps = 0;
			_lastActivatedTriggerIndex = -1;
		}

		_currentProgressSteps = Mathf.Clamp(_currentProgressSteps, 0, orderedTriggers.Count);
		if (IsComplete)
			HasCompletedOnce = true;

		UpdateCompletionRate();
		RefreshAndPublishPuzzleState();
	}

	private void HandleTriggerDeactivated(InteractiveObjectTrigger trigger)
	{
		if (orderedTriggers.Count == 0) return;

		if (!resetProgressAfterFinalExit)
		{
			UpdateCompletionRate();
			RefreshAndPublishPuzzleState();
			return;
		}

		// Once the sequence is complete, stepping off the final activated trigger resets it.
		if (_currentProgressSteps == orderedTriggers.Count && trigger.Index == _lastActivatedTriggerIndex)
		{
			_currentProgressSteps = 0;
			_lastActivatedTriggerIndex = -1;
		}

		UpdateCompletionRate();
		RefreshAndPublishPuzzleState();
	}

	private void UpdateCompletionRate()
	{
		if (orderedTriggers.Count == 0) return;

		CompletionRate = (float)_currentProgressSteps / orderedTriggers.Count;
	}

	public void ResetPuzzleState(bool clearHasCompletedOnce = false)
	{
		_currentProgressSteps = 0;
		_lastActivatedTriggerIndex = -1;

		if (clearHasCompletedOnce)
			HasCompletedOnce = false;

		UpdateCompletionRate();
		RefreshAndPublishPuzzleState();
	}

	public void ResetPuzzle()
	{
		ResetPuzzleState(true);
	}

	private void RegisterWithPuzzleManagerIfAvailable()
	{
		if (_isPuzzleRegistered)
			return;

		if (PuzzleManager.Instance == null)
			return;

		PuzzleManager.Instance.RegisterPuzzle(this);
		_isPuzzleRegistered = true;
	}

	private void UnregisterFromPuzzleManager()
	{
		if (!_isPuzzleRegistered)
			return;

		if (PuzzleManager.Instance != null)
			PuzzleManager.Instance.UnregisterPuzzle(this);

		_isPuzzleRegistered = false;
	}

	private void RefreshAndPublishPuzzleState()
	{
		RegisterWithPuzzleManagerIfAvailable();

		IPuzzle.PuzzleState nextState;
		if (HasCompletedOnce || IsComplete)
			nextState = IPuzzle.PuzzleState.Solved;
		else if (_currentProgressSteps > 0)
			nextState = IPuzzle.PuzzleState.InProgress;
		else
			nextState = IPuzzle.PuzzleState.Unsolved;

		State = nextState;

		if (PuzzleManager.Instance != null && !string.IsNullOrWhiteSpace(PuzzleId))
			PuzzleManager.Instance.UpdatePuzzleState(gameObject.scene.name, PuzzleId, State);
	}

	private string BuildDefaultPuzzleId()
	{
		if (transform == null)
			return gameObject.name;

		string path = transform.name;
		Transform current = transform.parent;

		while (current != null)
		{
			path = $"{current.name}/{path}";
			current = current.parent;
		}

		return path;
	}
}
