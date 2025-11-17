using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] float movementSpeed = 10f;
	
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
		Vector3 moveDirection = new Vector3(_inputValue.x, 0f, _inputValue.y);
		Vector3 newPosition = transform.position + moveDirection * (movementSpeed * Time.fixedDeltaTime);

		_rb.MovePosition(newPosition);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		_inputValue = context.ReadValue<Vector2>();
	}
	
}