using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
		UpdateRotation();
	}

	private void UpdateAnimations()
	{
		float currentHorizontalSpeed = new Vector3(_rb.linearVelocity.x, 0.0f, _rb.linearVelocity.z).magnitude;
		animator.SetFloat("MovementSpeed", currentHorizontalSpeed, 0.1f, Time.deltaTime);
	}

	private void UpdateRotation()
	{
		if (_rb.linearVelocity != Vector3.zero) _rb.MoveRotation(Quaternion.LookRotation(_rb.linearVelocity.normalized));
	}
}