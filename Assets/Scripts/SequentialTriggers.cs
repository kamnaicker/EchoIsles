using System;
using System.Collections.Generic;
using UnityEngine;

public class SequentialTriggers : MonoBehaviour
{
	[SerializeField] private List<InteractiveObjectTrigger> triggers;

	private int _completion;
	public float CompletionRate;

	private InteractiveObjectTrigger _firstTriggered;
	private InteractiveObjectTrigger _secondTriggered;

	private void Start()
	{
		for (int i = 0; i < triggers.Count; i++)
		{
			triggers[i].OnActivation += OnSomeTriggerActivation;
			triggers[i].OnDeactivation += OnSomeTriggerDeactivation;
			triggers[i].Index = i;
		}
	}

	private void OnSomeTriggerActivation(InteractiveObjectTrigger trigger)
	{
		if (_firstTriggered == null && _secondTriggered == null) _firstTriggered = trigger;

		if (_firstTriggered != null && _secondTriggered == null && _firstTriggered.Index != trigger.Index)
			_secondTriggered = trigger;

		ComputeCompletion();
		ComputeCompletionRate();
	}

	private void OnSomeTriggerDeactivation(InteractiveObjectTrigger trigger)
	{
		if (_firstTriggered?.Index == trigger.Index)
		{
			_firstTriggered = _secondTriggered;
			_secondTriggered = null;
		}

		if (_secondTriggered?.Index == trigger.Index) _secondTriggered = null;

		ComputeCompletion();
		ComputeCompletionRate();
	}

	private void ComputeCompletion()
	{
		// If none are triggerred - reset completion
		if (_firstTriggered == null && _secondTriggered == null)
		{
			_completion = 0;
			return;
		}

		// If both are triggered
		if (_firstTriggered != null && _secondTriggered != null)
		{
			// If an increment is detected, increase completion
			if (_firstTriggered.Index == _secondTriggered.Index - 1 &&
			    _secondTriggered.Index == _completion)
				_completion++;


			if (_firstTriggered.Index != _secondTriggered.Index - 1)
			{
				// If no increment is detected, do nothing
				if (_firstTriggered.Index + 1 == _completion || _secondTriggered.Index + 1 == _completion) return;

				// Otherwise (if two irrelevant triggers are active) reset completion
				_completion = 0;
			}
		}

		// If only one trigger is active
		if (_firstTriggered != null && _secondTriggered == null)
		{
			if (_firstTriggered.Index + 1 == _completion) return;

			// Handle first trigger, or decrement (e.g. when stepping off of the second pressure plate)
			if (_firstTriggered.Index == 0 || _firstTriggered.Index + 2 == _completion)
			{
				_completion = _firstTriggered.Index + 1;
				return;
			}

			_completion = 0;
		}
	}

	private void ComputeCompletionRate()
	{
		if (triggers.Count == 0) return;

		CompletionRate = (float)_completion / triggers.Count;
	}
}