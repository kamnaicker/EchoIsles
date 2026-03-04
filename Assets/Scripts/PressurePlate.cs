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
		// _renderer = model.GetComponent<Renderer>();
		// _defaultMaterial = _renderer.material;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
            // _renderer.material = pressedMaterial;
            AudioManager.Instance.HandlePressurePlateStateChanged(PressurePlateState.Down,
                gameObject.GetComponent<AudioSource>());
        Activate();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
            AudioManager.Instance.HandlePressurePlateStateChanged(PressurePlateState.Up,
                gameObject.GetComponent<AudioSource>());
        // _renderer.material = _defaultMaterial;
        Deactivate();
	}
}