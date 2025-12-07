using UnityEngine;

public class EchoArea : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) Replayer.Instance.ToggleRecordingAvailability(true);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) Replayer.Instance.ToggleRecordingAvailability(false);
	}
}