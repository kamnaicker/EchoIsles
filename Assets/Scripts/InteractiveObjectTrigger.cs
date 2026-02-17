using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjectTrigger : MonoBehaviour
{
	public event Action<InteractiveObjectTrigger> OnActivation;
	public event Action<InteractiveObjectTrigger> OnDeactivation;

	public int Index;

	protected void Activate()
	{
		OnActivation?.Invoke(this);
	}

	protected void Deactivate()
	{
		OnDeactivation?.Invoke(this);
	}
}