using System;
using UnityEngine;


public abstract class InteractiveObject : MonoBehaviour
{
	[SerializeField] private InteractiveObjectTrigger trigger;

	protected virtual void Start()
	{
		trigger.onActivation += OnActivation;
		trigger.onDeactivation += OnDeactivation;
	}

	protected abstract void OnActivation();
	protected abstract void OnDeactivation();
}