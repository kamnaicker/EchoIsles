using System;
using UnityEngine;

internal enum Direction
{
	Up,
	Down,
	None
}

public class Elevator : InteractiveObject
{
	[SerializeField] private float finalY = 3f;
	[SerializeField] private float speed = 2f;

	private Direction _direction = Direction.None;
	private float _initialY = 0f;

	private void Update()
	{
		HandleMovement();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected override void Start()
	{
		base.Start();
		_initialY = transform.position.y;
	}

	private void HandleMovement()
	{
		if (_direction == Direction.None) return;

		Vector3 finalPosition = new(transform.position.x, _direction == Direction.Up ? finalY : _initialY,
			transform.position.z);
		transform.position = Vector3.Lerp(transform.position, finalPosition,
			Time.deltaTime * speed);

		if (transform.position.y == _initialY) _direction = Direction.None;
	}

	protected override void OnActivation()
	{
		_direction = Direction.Up;

		Debug.Log("Elevator activated");
	}

	protected override void OnDeactivation()
	{
		_direction = Direction.Down;

		Debug.Log("Elevator deactivated");
	}
}