using UnityEngine;

public class Oscillator : MonoBehaviour
{
	[SerializeField] private Vector3 movementVector;
	[SerializeField] private float speed;

	private Vector3 _startPos;
	private Vector3 _endPos;
	private Rigidbody _rb;

	private float _movementFactor = 0;

	private void Start()
	{
		_startPos = transform.position;
		_endPos = _startPos + movementVector;
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		_movementFactor = Mathf.PingPong(Time.time * speed * Time.fixedDeltaTime, 1f);
		
		Vector3 nextPos = Vector3.Lerp(_startPos, _endPos, _movementFactor);
		
		if (_rb != null) 
		{
			_rb.MovePosition(nextPos);	
		}
		else
		{
			transform.position = nextPos;
		}
	}
}
