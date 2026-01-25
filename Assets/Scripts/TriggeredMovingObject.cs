using UnityEngine;
using System;
using UnityEngine;

internal enum MoveDirection
{
	Forward,
	Backward,
	None
}

public class TriggeredMovingObject : InteractiveObject
{
	[SerializeField] private Vector3 finalPositionOffset = Vector3.zero;
	[SerializeField] private float speed = 2f;

	private MoveDirection _direction = MoveDirection.None;
	private Vector3 _initialPosition = Vector3.zero;
	private Vector3 _finalPosition = Vector3.zero;

	private Rigidbody _rb;

	private void FixedUpdate()
	{
		HandleMovement();
	}

	protected override void Start()
	{
		base.Start();
		_rb = GetComponent<Rigidbody>();
		_initialPosition = transform.position;
		_finalPosition = _initialPosition + finalPositionOffset;
	}

	private void HandleMovement()
	{
		if (_direction == MoveDirection.None) return;

		Vector3 finalPosition = _direction == MoveDirection.Forward ? _finalPosition : _initialPosition;
		Vector3 newPosition = Vector3.Lerp(transform.position, finalPosition,
			Time.fixedDeltaTime * speed);

		_rb.MovePosition(newPosition);

		if (newPosition == _initialPosition) _direction = MoveDirection.None;
	}

	protected override void OnActivation()
	{
		_direction = MoveDirection.Forward;
	}

	protected override void OnDeactivation()
	{
		_direction = MoveDirection.Backward;
	}
}