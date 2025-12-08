using System;
using UnityEngine;

public class PressurePlate : InteractiveObjectTrigger
{
	[SerializeField] private Material pressedMaterial;
	[SerializeField] private GameObject model;

	private Material _defaultMaterial;
	private Renderer _renderer;

	private void Awake()
	{
		_renderer = model.GetComponent<Renderer>();
		_defaultMaterial = _renderer.material;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_renderer.material = pressedMaterial;
			Activate();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_renderer.material = _defaultMaterial;
			Deactivate();
		}
	}
}