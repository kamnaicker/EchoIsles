using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjectTrigger : MonoBehaviour
{
	public event Action OnActivation;
	public event Action OnDeactivation;

	protected void Activate()
	{
		OnActivation?.Invoke();
	}

	protected void Deactivate()
	{
		OnDeactivation?.Invoke();
	}
}