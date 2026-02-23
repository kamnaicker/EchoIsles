using UnityEngine;

public class SequentialMovingObject : MonoBehaviour
{
	[SerializeField] private Vector3 finalPositionOffset = Vector3.zero;
	[SerializeField] private float speed = 2f;
	[SerializeField] private float smoothTime = 0.3f;

	[SerializeField] private SequentialTriggers sequentialTriggers;

	private Vector3 _initialPosition = Vector3.zero;
	private Vector3 _finalPosition = Vector3.zero;
	private Vector3 _currentVelocity;


	private void FixedUpdate()
	{
		HandleMovement();
	}

	protected void Start()
	{
		_initialPosition = transform.position;
		_finalPosition = _initialPosition + finalPositionOffset;
	}

	private void HandleMovement()
	{
		Vector3 targetPosition = Vector3.Lerp(_initialPosition, _finalPosition,
			sequentialTriggers.CompletionRate);

		transform.position = Vector3.SmoothDamp(
			transform.position,
			targetPosition,
			ref _currentVelocity,
			smoothTime
		);
	}
}