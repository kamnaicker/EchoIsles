using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjectTrigger : MonoBehaviour
{
	public event Action onActivation;
	public event Action onDeactivation;

	protected void Activate()
	{
		onActivation?.Invoke();
	}

	protected void Deactivate()
	{
		onDeactivation?.Invoke();
	}
}