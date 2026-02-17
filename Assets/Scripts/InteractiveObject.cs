using System;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
	[SerializeField] private InteractiveObjectTrigger trigger;

	protected virtual void Start()
	{
		trigger.OnActivation += OnActivation;
		trigger.OnDeactivation += OnDeactivation;
	}

	protected abstract void OnActivation(InteractiveObjectTrigger trigger);
	protected abstract void OnDeactivation(InteractiveObjectTrigger trigger);
}