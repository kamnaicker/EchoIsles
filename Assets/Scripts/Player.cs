using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
	public float MoveSpeed = 4.0f;
	public float SpeedChangeRate = 10.0f;
	public float Gravity = -15.0f;

	public float FallTimeout = 0.15f;

	public bool IsGrounded = true;
	public float GroundedOffset = -0.14f;
	public float GroundedRadius = 0.5f;
	public LayerMask GroundLayers;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	private float _fallTimeoutDelta;

	private CharacterController _controller;
	private Vector2 _inputValue;

	public void OnMove(InputAction.CallbackContext context)
	{
		_inputValue = context.ReadValue<Vector2>();
	}

	private void Start()
	{
		_controller = GetComponent<CharacterController>();

		_fallTimeoutDelta = FallTimeout;
	}

	private void FixedUpdate()
	{
		ApplyGravity();
		GroundedCheck();
		Move();
	}

	private void GroundedCheck()
	{
		Vector3 spherePosition = new(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

	private void Move()
	{
		float targetSpeed = MoveSpeed;

		if (_inputValue == Vector2.zero) targetSpeed = 0.0f;

		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = 1f;

		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		Vector3 inputDirection = new Vector3(_inputValue.x, 0.0f, _inputValue.y).normalized;

		if (_inputValue != Vector2.zero)
			inputDirection = transform.right * _inputValue.x + transform.forward * _inputValue.y;

		_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
		                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void ApplyGravity()
	{
		if (IsGrounded)
		{
			_fallTimeoutDelta = FallTimeout;

			if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
		}
		else
		{
			if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.deltaTime;
		}

		if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Gravity * Time.deltaTime;
	}


	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

		if (IsGrounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
			GroundedRadius);
	}
}