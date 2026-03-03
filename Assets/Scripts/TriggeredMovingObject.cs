using UnityEngine;
using UnityEngine.Serialization;

internal enum MovementDirection
{
	Forward,
	Backward,
	None
}

public class TriggeredMovingObject : InteractiveObject, IPuzzle
{
	[SerializeField] private string puzzleId;
	[FormerlySerializedAs("registerAsPuzzle")]
	[SerializeField] private bool reportToPuzzleManager = true;
	[FormerlySerializedAs("keepSolvedAfterReturning")]
	[SerializeField] private bool retainSolvedStateWhenReturning = true;
	[FormerlySerializedAs("arrivalThreshold")]
	[SerializeField] private float arrivalDistanceThreshold = 0.01f;

	[FormerlySerializedAs("finalPositionOffset")]
	[SerializeField] private Vector3 targetPositionOffset = Vector3.zero;
	[FormerlySerializedAs("speed")]
	[SerializeField] private float moveSpeed = 2f;

	private MovementDirection _movementDirection = MovementDirection.None;
	private Vector3 _initialPosition = Vector3.zero;
	private Vector3 _targetPosition = Vector3.zero;

	private Rigidbody _rigidbody;
	private bool _isPuzzleRegistered;
	private bool _hasReachedSolvedState;

	public string PuzzleId => string.IsNullOrWhiteSpace(puzzleId) ? BuildHierarchyPuzzleId() : puzzleId;
	public IPuzzle.PuzzleState State { get; private set; } = IPuzzle.PuzzleState.Unsolved;

	private void OnEnable()
	{
		RegisterWithPuzzleManagerIfAvailable();
		PublishStateToPuzzleManager(force: true);
	}

	private void OnDisable()
	{
		UnregisterFromPuzzleManager();
	}

	private void FixedUpdate()
	{
		HandleMovement();
	}

	protected override void Start()
	{
		base.Start();
		_rigidbody = GetComponent<Rigidbody>();
		_initialPosition = transform.position;
		_targetPosition = _initialPosition + targetPositionOffset;
		SetPuzzleState(IPuzzle.PuzzleState.Unsolved);
		PublishStateToPuzzleManager(force: true);
	}

	private void HandleMovement()
	{
		if (_movementDirection == MovementDirection.None) return;

		bool movingTowardActivatedPosition = _movementDirection == MovementDirection.Forward;
		Vector3 movementTarget = movingTowardActivatedPosition ? _targetPosition : _initialPosition;
		Vector3 currentPosition = _rigidbody != null ? _rigidbody.position : transform.position;
		Vector3 nextPosition = Vector3.MoveTowards(currentPosition, movementTarget, moveSpeed * Time.fixedDeltaTime);

		if (_rigidbody != null)
			_rigidbody.MovePosition(nextPosition);
		else
			transform.position = nextPosition;

		float distanceToTarget = Vector3.Distance(nextPosition, movementTarget);
		if (distanceToTarget > arrivalDistanceThreshold)
		{
			if (!(_hasReachedSolvedState && retainSolvedStateWhenReturning))
				SetPuzzleState(IPuzzle.PuzzleState.InProgress);
			return;
		}

		if (_rigidbody != null)
			_rigidbody.MovePosition(movementTarget);
		else
			transform.position = movementTarget;

		_movementDirection = MovementDirection.None;

		if (movingTowardActivatedPosition)
		{
			_hasReachedSolvedState = true;
			SetPuzzleState(IPuzzle.PuzzleState.Solved);
			return;
		}

		if (_hasReachedSolvedState && retainSolvedStateWhenReturning)
			SetPuzzleState(IPuzzle.PuzzleState.Solved);
		else
			SetPuzzleState(IPuzzle.PuzzleState.Unsolved);
	}

	protected override void OnActivation(InteractiveObjectTrigger trigger)
	{
		_movementDirection = MovementDirection.Forward;
		if (!(_hasReachedSolvedState && retainSolvedStateWhenReturning))
			SetPuzzleState(IPuzzle.PuzzleState.InProgress);
	}

	protected override void OnDeactivation(InteractiveObjectTrigger trigger)
	{
		if (!(_hasReachedSolvedState && retainSolvedStateWhenReturning))
		{
			_movementDirection = MovementDirection.Backward;
			SetPuzzleState(IPuzzle.PuzzleState.InProgress);
		}
	}

	public void ResetPuzzle()
	{
		_hasReachedSolvedState = false;
		_movementDirection = MovementDirection.None;

		if (_rigidbody != null)
			_rigidbody.position = _initialPosition;
		else
			transform.position = _initialPosition;

		SetPuzzleState(IPuzzle.PuzzleState.Unsolved);
	}

	private void RegisterWithPuzzleManagerIfAvailable()
	{
		if (_isPuzzleRegistered)
			return;

		if (!reportToPuzzleManager || PuzzleManager.Instance == null)
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

	private void SetPuzzleState(IPuzzle.PuzzleState nextState)
	{
		if (State == nextState)
			return;

		State = nextState;
		PublishStateToPuzzleManager();
	}

	private void PublishStateToPuzzleManager(bool force = false)
	{
		RegisterWithPuzzleManagerIfAvailable();

		if (!reportToPuzzleManager || PuzzleManager.Instance == null)
			return;

		if (string.IsNullOrWhiteSpace(PuzzleId))
			return;

		if (force || _isPuzzleRegistered)
			PuzzleManager.Instance.UpdatePuzzleState(gameObject.scene.name, PuzzleId, State);
	}

	private string BuildHierarchyPuzzleId()
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
