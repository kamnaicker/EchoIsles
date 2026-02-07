using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
	public float MoveSpeed = 4.0f;
	public float SpeedChangeRate = 10.0f;
	public float Gravity = -15.0f;

	public float FallTimeout = 0.15f;

	public bool IsGrounded = true;
	public float GroundedOffset = -0.14f;
	public float GroundedRadius = 0.5f;
	public float rotationSpeed = 10f;
	public LayerMask GroundLayers;

	[SerializeField] private Animator animator;

	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	private GameObject _parentPlatform;

	private float _fallTimeoutDelta;

	private CharacterController _controller;
	private Vector2 _inputValue;

	private Vector3 _platformPosition;
	private Vector3 _platformMovement;

	public void OnMove(InputAction.CallbackContext context)
	{
		_inputValue = context.ReadValue<Vector2>();
	}

	private void Start()
	{
		_controller = GetComponent<CharacterController>();

		_fallTimeoutDelta = FallTimeout;
	}

	private void Update()
	{
		UpdateAnimations();
	}

	private void FixedUpdate()
	{
		if (!_parentPlatform) ApplyGravity();
		ApplyPlatformMovement();

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
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.fixedDeltaTime * SpeedChangeRate);
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		Vector3 inputDirection = new Vector3(_inputValue.x, 0.0f, _inputValue.y).normalized;

		if (_inputValue != Vector2.zero)
			inputDirection = Vector3.right * _inputValue.x + Vector3.forward * _inputValue.y;

		Vector3 inputMovement = _speed * Time.fixedDeltaTime * inputDirection.normalized;
		Vector3 gravityMovement = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime;

		// TODO: Fix platform behaviour
		Vector3 totalMovement = _parentPlatform && _controller.isGrounded
			? inputMovement + _platformMovement
			: inputMovement + gravityMovement;

		_controller.Move(totalMovement);


		Vector3 targetRotationDirection = _controller.velocity.normalized;

		if (targetRotationDirection == Vector3.zero) targetRotationDirection = transform.forward;

		Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
		Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
		transform.rotation = targetRotation;
	}

	private void UpdateAnimations()
	{
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
		animator.SetFloat("MovementSpeed", currentHorizontalSpeed, 0.1f, Time.deltaTime);
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
			if (_fallTimeoutDelta >= 0.0f) _fallTimeoutDelta -= Time.fixedDeltaTime;
		}

		if (_verticalVelocity < _terminalVelocity) _verticalVelocity += Gravity;
	}

	private void ApplyPlatformMovement()
	{
		if (!_parentPlatform) return;
		if (_parentPlatform.transform.position.Equals(_platformPosition)) return;

		_platformMovement = _parentPlatform.transform.position - _platformPosition;
		_platformPosition = _parentPlatform.transform.position;
	}

	// private void OnControllerColliderHit(ControllerColliderHit hit)
	// {
	// 	float incline = 0.9f;
	// 	if (hit.normal.y > incline)
	// 	{
	// 		Debug.Log(hit.normal);
	//
	// 		GameObject other = hit.collider.gameObject;
	//
	// 		if (other.CompareTag("Platform"))
	// 		{
	// 			gameObject.transform.parent = other.transform;
	// 		}
	// 	}
	// }


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Platform"))
		{
			_parentPlatform = other.gameObject;
			_platformPosition = _parentPlatform.transform.position;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Platform"))
		{
			_parentPlatform = null;
			_platformMovement = Vector3.zero;
		}
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