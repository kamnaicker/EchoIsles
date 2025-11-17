using System;
using UnityEngine;

public class PressurePlate : InteractiveObjectTrigger
{
	[SerializeField] Material pressedMaterial;
	
	Material _defaultMaterial;	
	Renderer _renderer;

	private void Awake()
	{
		_renderer = GetComponent<Renderer>();
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
		    _renderer.material =  _defaultMaterial;
		    Deactivate();
	    }
    }
}
