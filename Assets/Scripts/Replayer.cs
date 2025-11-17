using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Replayer : MonoBehaviour
{
	public static Replayer Instance;

	private List<Vector3> _positions;
	public Transform player;
	public Transform echo;
	[SerializeField] private bool recording = false;
	[SerializeField] private bool replaying = false;

	private void Awake()
	{
		// There can only be one WorldSaveGameManager. If there is already an instance, destroy this one.
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void Start()
	{
		DontDestroyOnLoad(gameObject);
		_positions = new List<Vector3>();
	}


	private void FixedUpdate()
	{
		if (recording) Record();

		if (replaying) Replay();
	}

	private void Record()
	{
		_positions.Insert(0, player.position);
	}

	private void Replay()
	{
		if (_positions.Count == 0)
		{
			replaying = false;
			return;
		}

		echo.position = _positions.LastOrDefault();
		_positions.RemoveAt(_positions.Count - 1);
	}
}