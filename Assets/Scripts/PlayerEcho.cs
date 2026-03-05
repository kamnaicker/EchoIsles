using System;
using UnityEngine;

public class PlayerEcho : MonoBehaviour
{
	private Rigidbody _rb;
	[SerializeField] private Animator animator;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		UpdateAnimations();
	}

	private void UpdateAnimations()
	{
		float currentHorizontalSpeed = new Vector3(_rb.linearVelocity.x, 0.0f, _rb.linearVelocity.z).magnitude;
		animator.SetFloat("MovementSpeed", currentHorizontalSpeed, 0.1f, Time.deltaTime);
	}
}