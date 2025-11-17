using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] private float movementSpeed = 10f;

	[SerializeField] protected float gravityForce = -9.81f;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private float groundCheckSphereRadius = 1f;
	[SerializeField] protected float groundedYVelocity = -20f;
	[SerializeField] protected float fallStartYVelocity = -5f;
	protected bool fallingVelocityHasBeenSet;
	protected float inAirTimer;
	private bool isGrounded;
	protected Vector3 yVelocity;


	private Vector2 _inputValue;
	private Rigidbody _rb;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		Vector3 moveDirection = new(_inputValue.x, 0f, _inputValue.y);
		Vector3 newPosition = transform.position + moveDirection * (movementSpeed * Time.fixedDeltaTime);

		_rb.MovePosition(newPosition);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		_inputValue = context.ReadValue<Vector2>();
	}

	// TODO: Implement gravity
	// protected virtual void Update()
	// {
	// 	HandleGroundCheck();
	//
	// 	if (isGrounded)
	// 	{
	// 		if (yVelocity.y < 0)
	// 		{
	// 			inAirTimer = 0;
	// 			fallingVelocityHasBeenSet = false;
	// 			yVelocity.y = groundedYVelocity;
	// 		}
	// 	}
	// 	else
	// 	{
	// 		fallingVelocityHasBeenSet = true;
	// 		yVelocity.y = fallStartYVelocity;
	//
	// 		inAirTimer += Time.deltaTime;
	// 		yVelocity.y += gravityForce * Time.deltaTime;
	// 	}
	//
	// 	// There should always be some force applied to the character
	// 	// characterController.Move(yVelocity * Time.deltaTime);
	// 	transform.Translate(yVelocity * Time.deltaTime, Space.World);
	// }
	//
	// protected void OnDrawGizmosSelected()
	// {
	// 	Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
	// }
	//
	// protected void HandleGroundCheck()
	// {
	// 	isGrounded = Physics.CheckSphere(transform.position,
	// 		groundCheckSphereRadius, groundLayer);
	// }
}